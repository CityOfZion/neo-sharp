using NeoSharp.Core.Network.Messages;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network.Tcp.Protocols
{
    public class TcpProtocolV2 : ITcpProtocol
    {
        public override async Task SendMessageAsync(NetworkStream stream, Message message, CancellationTokenSource cancellationToken)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8))
            {
                writer.Write((byte)message.Flags);
                writer.Write((byte)message.Command);
                writer.Write((uint)message.RawPayload.Length);
                writer.Write(message.RawPayload);

                byte[] buffer = ms.ToArray();
                await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken.Token);
            }
        }

        public override async Task<Message> GetMessageAsync(NetworkStream stream, CancellationTokenSource cancellationToken)
        {
            uint payload_length;
            Message message = new Message();

            byte[] buffer = await FillBufferAsync(stream, 8, cancellationToken.Token);

            using (MemoryStream ms = new MemoryStream(buffer, false))
            using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
            {
                message.Flags = (MessageFlags)reader.ReadByte();
                message.Command = (MessageCommand)reader.ReadByte();
                payload_length = reader.ReadUInt32();

                if (payload_length > Message.PayloadMaxSize)
                    throw new FormatException();
            }

            if (payload_length > 0)
                message.RawPayload = await FillBufferAsync(stream, (int)payload_length, cancellationToken.Token);
            else
                message.RawPayload = new byte[0];

            return message;
        }
    }
}