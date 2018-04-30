namespace NeoSharp.Core.Network.Messages
{
    public enum MessageFlags : byte
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