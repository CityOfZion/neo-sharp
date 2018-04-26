namespace NeoSharp.Core.Network.Tcp
{
    public class TcpMessage
    {
        /// <summary>
        /// Max size for payload
        /// </summary>
        public const int PayloadMaxSize = 0x02000000;

        /// <summary>
        /// Flags
        /// </summary>
        public TcpMessageFlags Flags;
        /// <summary>
        /// Command
        /// </summary>
        public TcpMessageCommand Command;
        /// <summary>
        /// Payload
        /// </summary>
        public byte[] Payload;
    }
}