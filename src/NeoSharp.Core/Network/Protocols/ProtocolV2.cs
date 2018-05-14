using NeoSharp.BinarySerialization;
using NeoSharp.Core.Messaging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network.Protocols
{
    public class ProtocolV2 : ProtocolBase
    {
        private readonly IBinaryConverter _serializer;
        private readonly uint _magic;

        public ProtocolV2(NetworkConfig config, IBinaryConverter serializer)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            _magic = config.Magic;
        }

        public override uint Version => 2;

        public override bool IsHighPriorityMessage(Message m)
        {
            return m.Flags.HasFlag(MessageFlags.Urgent) || base.IsHighPriorityMessage(m);
        }

        public override async Task SendMessageAsync(Stream stream, Message message,
            CancellationToken cancellationToken)
        {
            using (var memory = new MemoryStream())
            using (var writer = new BinaryWriter(memory, Encoding.UTF8))
            {
                writer.Write(_magic);
                writer.Write((byte)message.Command);
                writer.Write((byte)message.Flags);

                var payloadBuffer = message is ICarryPayload messageWithPayload
                    ? _serializer.Serialize(messageWithPayload.Payload)
                    : new byte[0];

                writer.Write((uint)payloadBuffer.Length);
                writer.Write(payloadBuffer);
                writer.Flush();

                var buffer = memory.ToArray();
                await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            }
        }

        public override async Task<Message> ReceiveMessageAsync(Stream stream, CancellationToken cancellationToken)
        {
            var buffer = await FillBufferAsync(stream, 10, cancellationToken);

            using (var memory = new MemoryStream(buffer, false))
            using (var reader = new BinaryReader(memory, Encoding.UTF8))
            {
                if (reader.ReadUInt32() != _magic)
                    throw new FormatException();

                var command = (MessageCommand)reader.ReadByte();

                if (!Cache.TryGetValue(command, out var type))
                    throw (new ArgumentException("command"));

                var message = (Message)Activator.CreateInstance(type);

                message.Command = command;
                message.Flags = (MessageFlags)reader.ReadByte();

                var payloadLength = reader.ReadUInt32();
                if (payloadLength > Message.PayloadMaxSize)
                    throw new FormatException();

                var payloadBuffer = payloadLength > 0
                    ? await FillBufferAsync(stream, (int)payloadLength, cancellationToken)
                    : new byte[0];

                if (message is ICarryPayload messageWithPayload)
                {
                    if (payloadLength == 0)
                        throw new FormatException();

                    _serializer.Deserialize(payloadBuffer, messageWithPayload.Payload);
                }

                return message;
            }
        }
    }
}