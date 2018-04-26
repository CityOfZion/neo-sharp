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
        public override async Task<TcpMessage> GetMessageAsync(NetworkStream stream, CancellationTokenSource cancellationToken)
        {
            ushort checksum;
            uint payload_length;
            TcpMessage message = new TcpMessage();

            byte[] buffer = await FillBufferAsync(stream, 8, cancellationToken.Token);

            using (MemoryStream ms = new MemoryStream(buffer, false))
            using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
            {
                message.Flags = (TcpMessageFlags)reader.ReadByte();
                message.Command = (TcpMessageCommand)reader.ReadByte();
                checksum = reader.ReadUInt16();
                payload_length = reader.ReadUInt32();

                if (payload_length > TcpMessage.PayloadMaxSize)
                    throw new FormatException();
            }

            if (payload_length > 0)
                message.Payload = await FillBufferAsync(stream, (int)payload_length, cancellationToken.Token);
            else
                message.Payload = new byte[0];

            if (GetChecksum(message.Payload) != checksum)
                throw new FormatException();

            return message;
        }

        #region CRC

        const ushort polynomial = 0xA001;
        static readonly ushort[] table = new ushort[256];

        static TcpProtocolV2()
        {
            ushort temp;
            ushort value;

            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }

        private static ushort GetChecksum(byte[] value)
        {
            ushort crc = 0;

            for (int i = 0; i < value.Length; ++i)
            {
                byte index = (byte)(crc ^ value[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }

            return crc;
        }

        #endregion
    }
}