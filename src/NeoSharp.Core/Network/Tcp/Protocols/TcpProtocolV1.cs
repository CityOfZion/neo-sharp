using NeoSharp.Core.Extensions;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network.Tcp.Protocols
{
    public class TcpProtocolV1 : ITcpProtocol
    {
        public override async Task<TcpMessage> GetMessageAsync(NetworkStream stream, CancellationTokenSource cancellationToken)
        {
            uint payload_length, checksum;
            TcpMessage message = new TcpMessage()
            {
                Flags = TcpMessageFlags.None
            };

            byte[] buffer = await FillBufferAsync(stream, 24, cancellationToken.Token);

            using (MemoryStream ms = new MemoryStream(buffer, false))
            using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
            {
                if (reader.ReadUInt32() != MagicHeader)
                    throw new FormatException();

                byte[] data = reader.ReadBytes(12);
                message.Command = Enum.Parse<TcpMessageCommand>(Encoding.UTF8.GetString(data));
                payload_length = reader.ReadUInt32();

                if (payload_length > TcpMessage.PayloadMaxSize)
                    throw new FormatException();

                checksum = reader.ReadUInt32();
            }

            if (payload_length > 0)
                message.Payload = await FillBufferAsync(stream, (int)payload_length, cancellationToken.Token);
            else
                message.Payload = new byte[0];

            if (GetChecksum(message.Payload) != checksum)
                throw new FormatException();

            return message;
        }

        private static uint GetChecksum(byte[] value)
        {
            return value.Sha256(0, value.Length).Sha256(0, 32).ToUInt32(0);
        }
    }
}