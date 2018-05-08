namespace NeoSharp.Core.Messaging
{
    public class Message
    {
        /// <summary>
        /// Max size for payload
        /// </summary>
        public const int PayloadMaxSize = 0x02000000;

        /// <summary>
        /// Flags
        /// </summary>
        public MessageFlags Flags;

        /// <summary>
        /// Command
        /// </summary>
        public MessageCommand Command;
    }
}