using NeoSharp.BinarySerialization;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network.Tcp.Protocols
{
    public class TcpProtocolV1 : TcpProtocolBase
    {
        public override uint MagicHeader => 1;

        public override async Task SendMessageAsync(Stream stream, Message message,
            CancellationToken cancellationToken)
        {
            using (var memory = new MemoryStream())
            using (var writer = new BinaryWriter(memory, Encoding.UTF8))
            {
                writer.Write(MagicHeader);
                writer.Write(Encoding.UTF8.GetBytes(message.Command.ToString().PadRight(12, ' ')), 0, 12);

                var payloadBuffer = message is ICarryPayload messageWithPayload
                    ? BinarySerializer.Serialize(messageWithPayload.Payload)
                    : new byte[0];

                writer.Write(payloadBuffer.Length);
                writer.Write(CalculateChecksum(payloadBuffer));
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
                if (reader.ReadUInt32() != MagicHeader)
                    throw new FormatException();

                MessageCommand command = Enum.Parse<MessageCommand>(Encoding.UTF8.GetString(reader.ReadBytes(12)));

                if (!Cache.TryGetValue(command, out Type type))
                    throw (new ArgumentException("command"));

                var message = (Message)Activator.CreateInstance(type);
                message.Command = command;

                var payloadLength = reader.ReadUInt32();
                if (payloadLength > Message.PayloadMaxSize)
                    throw new FormatException();

                var checksum = reader.ReadUInt32();

                var payloadBuffer = payloadLength > 0
                    ? await FillBufferAsync(stream, (int)payloadLength, cancellationToken)
                    : new byte[0];

                if (CalculateChecksum(payloadBuffer) != checksum)
                    throw new FormatException();

                if (message is ICarryPayload messageWithPayload)
                {
                    if (payloadLength == 0)
                        throw new FormatException();

                    messageWithPayload.Payload = BinarySerializer.Deserialize(payloadBuffer, messageWithPayload.Payload.GetType());
                }

                return message;
            }
        }

        private static uint CalculateChecksum(byte[] value)
        {
            return value.Sha256(0, value.Length).Sha256(0, 32).ToUInt32(0);
        }
    }
}