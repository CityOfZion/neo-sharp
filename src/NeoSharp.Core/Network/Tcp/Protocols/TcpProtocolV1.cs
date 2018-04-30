using NeoSharp.Core.Extensions;
using NeoSharp.Core.Network.Messages;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Network.Messaging;

namespace NeoSharp.Core.Network.Tcp.Protocols
{
    public class TcpProtocolV1 : TcpProtocolBase
    {
        public override async Task SendMessageAsync(NetworkStream stream, Message message,
            CancellationToken cancellationToken)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms, Encoding.UTF8))
            {
                writer.Write(MagicHeader);
                writer.Write(Encoding.UTF8.GetBytes(message.Command.ToString().PadRight(12, ' ')), 0, 12);
                writer.Write(message.RawPayload.Length);
                writer.Write(GetChecksum(message.RawPayload));
                writer.Write(message.RawPayload);

                var buffer = ms.ToArray();
                await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            }
        }

        public override async Task<Message> ReceiveMessageAsync(NetworkStream stream, CancellationToken cancellationToken)
        {
            uint payloadLength, checksum;
            var message = new Message
            {
                Flags = MessageFlags.None
            };

            var buffer = await FillBufferAsync(stream, 24, cancellationToken);

            using (var ms = new MemoryStream(buffer, false))
            using (var reader = new BinaryReader(ms, Encoding.UTF8))
            {
                if (reader.ReadUInt32() != MagicHeader)
                    throw new FormatException();

                var data = reader.ReadBytes(12);
                message.Command = Enum.Parse<MessageCommand>(Encoding.UTF8.GetString(data));
                payloadLength = reader.ReadUInt32();

                if (payloadLength > Message.PayloadMaxSize)
                    throw new FormatException();

                checksum = reader.ReadUInt32();
            }

            if (payloadLength > 0)
                message.RawPayload = await FillBufferAsync(stream, (int)payloadLength, cancellationToken);
            else
                message.RawPayload = new byte[0];

            if (GetChecksum(message.RawPayload) != checksum)
                throw new FormatException();

            return message;
        }

        private static uint GetChecksum(byte[] value)
        {
            return value.Sha256(0, value.Length).Sha256(0, 32).ToUInt32(0);
        }
    }
}