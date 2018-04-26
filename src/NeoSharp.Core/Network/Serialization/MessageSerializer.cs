using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Network.Messages;

namespace NeoSharp.Core.Network.Serialization
{
    public class MessageSerializer : IMessageSerializer
    {
        // TODO: Move to config
        private const int PayloadMaxSize = 0x02000000;

        // TODO: Move to config
        private static readonly uint Magic = 7630401;

        public async Task SerializeTo<TMessage>(TMessage message, Stream stream, CancellationToken cancellationToken)
            where TMessage : Message
        {
            using (var memory = new MemoryStream())
            using (var writer = new BinaryWriter(memory, Encoding.UTF8))
            {
                writer.Write(Magic);
                writer.WriteFixedString(message.Command, 12);

                byte[] payloadBuffer;

                if (message is ICarryPayload messageWithPayload)
                {
                    payloadBuffer = SerializeAsBytes(messageWithPayload.Payload);
                }
                else
                {
                    payloadBuffer = new byte[0];
                }

                writer.Write(payloadBuffer.Length);
                writer.Write(CalculateChecksum(payloadBuffer));
                writer.Write(payloadBuffer);

                writer.Flush();

                var buffer = memory.ToArray();
                await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            }
        }

        public async Task<TMessage> DeserializeFrom<TMessage>(
            Stream stream,
            CancellationToken cancellationToken)
            where TMessage : Message, new()
        {
            var message = new TMessage();
            var buffer = await FillBuffer(stream, 24, cancellationToken);

            using (var memory = new MemoryStream(buffer, false))
            using (var reader = new BinaryReader(memory, Encoding.UTF8))
            {
                if (reader.ReadUInt32() != Magic)
                    throw new FormatException();

                var command = reader.ReadFixedString(12);
                if (message.Command != command)
                    throw new FormatException();

                var payloadLength = reader.ReadUInt32();
                if (payloadLength > PayloadMaxSize)
                    throw new FormatException();

                var payloadBuffer = payloadLength > 0
                    ? await FillBuffer(stream, (int)payloadLength, cancellationToken)
                    : new byte[0];

                var checksum = reader.ReadUInt32();

                if (CalculateChecksum(payloadBuffer) != checksum)
                    throw new FormatException();

                if (message is ICarryPayload messageWithPayload)
                {
                    if (payloadLength == 0)
                        throw new FormatException();

                    DeserializeAsBytes(payloadBuffer, messageWithPayload.Payload);
                }
            }

            return message;
        }

        private static byte[] SerializeAsBytes(IBinarySerializable obj)
        {
            using (var memory = new MemoryStream())
            using (var writer = new BinaryWriter(memory, Encoding.UTF8))
            {
                obj.Serialize(writer);
                writer.Flush();

                return memory.ToArray();
            }
        }

        private static void DeserializeAsBytes(byte[] buffer, IBinarySerializable obj)
        {
            using (var memory = new MemoryStream(buffer, false))
            using (var reader = new BinaryReader(memory, Encoding.UTF8))
            {
                obj.Deserialize(reader);
            }
        }

        private static async Task<byte[]> FillBuffer(
            Stream stream,
            int size,
            CancellationToken cancellationToken)
        {
            size = Math.Min(size, 1024);
            var buffer = new byte[size];

            using (var memory = new MemoryStream())
            {
                while (size > 0)
                {
                    var count = Math.Min(size, buffer.Length);

                    count = await stream.ReadAsync(buffer, 0, count, cancellationToken);
                    if (count <= 0) throw new IOException();

                    memory.Write(buffer, 0, count);
                    size -= count;
                }

                return memory.ToArray();
            }
        }

        private uint CalculateChecksum(byte[] buffer)
        {
            // TODO: Get Crypto as a dependency and invoke real checksum
            return (uint)buffer.Length;
        }
    }
}