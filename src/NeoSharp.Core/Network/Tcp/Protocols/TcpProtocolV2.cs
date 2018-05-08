using NeoSharp.BinarySerialization;
using NeoSharp.Core.Messaging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network.Tcp.Protocols
{
    public class TcpProtocolV2 : TcpProtocolBase
    {
        public override uint MagicHeader => 2;

        public override async Task SendMessageAsync(Stream stream, Message message,
            CancellationToken cancellationToken)
        {
            using (var memory = new MemoryStream())
            using (var writer = new BinaryWriter(memory, Encoding.UTF8))
            {
                writer.Write((byte)message.Command);
                writer.Write((byte)message.Flags);

                var payloadBuffer = message is ICarryPayload messageWithPayload
                    ? BinarySerializer.Serialize(messageWithPayload.Payload)
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
            using (var reader = new BinaryReader(stream, Encoding.UTF8))
            {
                MessageCommand command = (MessageCommand)reader.ReadByte();

                if (!Cache.TryGetValue(command, out Type type))
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

                    messageWithPayload.Payload = BinarySerializer.Deserialize(payloadBuffer, messageWithPayload.Payload.GetType());
                }

                return message;
            }
        }
    }
}