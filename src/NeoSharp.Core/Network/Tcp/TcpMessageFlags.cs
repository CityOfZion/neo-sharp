namespace NeoSharp.Core.Network.Tcp
{
    public enum TcpMessageFlags : byte
    {
        /// <summary>
        /// No flag
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Compressed
        /// </summary>
        Compressed = 0x01,
        /// <summary>
        /// Urgent
        /// </summary>
        Urgent = 0x02,
    }
}