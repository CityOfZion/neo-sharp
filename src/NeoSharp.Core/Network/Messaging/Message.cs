using NeoSharp.Core.Serializers;

namespace NeoSharp.Core.Network.Messaging
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
        [BinaryProperty(0)]
        public MessageFlags Flags;
        /// <summary>
        /// Command
        /// </summary>
        [BinaryProperty(1)]
        public MessageCommand Command;
        /// <summary>
        /// Payload
        /// </summary>
        [BinaryProperty(2, MaxLength = PayloadMaxSize)]
        public byte[] RawPayload;
    }
}