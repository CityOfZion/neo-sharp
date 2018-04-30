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
    public class TcpProtocolV2 : TcpProtocolBase
    {
        public override async Task SendMessageAsync(NetworkStream stream, Message message,
            CancellationToken cancellationToken)
        {
            using (var ms = new MemoryStream())
            using (var writer = new BinaryWriter(ms, Encoding.UTF8))
            {
                writer.Write((byte)message.Flags);
                writer.Write((byte)message.Command);
                writer.Write((uint)message.RawPayload.Length);
                writer.Write(message.RawPayload);

                var buffer = ms.ToArray();
                await stream.WriteAsync(buffer, 0, buffer.Length, cancellationToken);
            }
        }

        public override async Task<Message> ReceiveMessageAsync(NetworkStream stream, CancellationToken cancellationToken)
        {
            uint payloadLength;
            var message = new Message();

            var buffer = await FillBufferAsync(stream, 8, cancellationToken);

            using (var ms = new MemoryStream(buffer, false))
            using (var reader = new BinaryReader(ms, Encoding.UTF8))
            {
                message.Flags = (MessageFlags)reader.ReadByte();
                message.Command = (MessageCommand)reader.ReadByte();
                payloadLength = reader.ReadUInt32();

                if (payloadLength > Message.PayloadMaxSize)
                    throw new FormatException();
            }

            if (payloadLength > 0)
                message.RawPayload = await FillBufferAsync(stream, (int)payloadLength, cancellationToken);
            else
                message.RawPayload = new byte[0];

            return message;
        }
    }
}