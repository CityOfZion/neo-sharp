using NeoSharp.BinarySerialization;

namespace NeoSharp.Core.Messaging.Messages
{
    public class VersionMessage : Message<VersionPayload>
    {
        /// <summary>
        /// Constructor necessary to be used by the serializer
        /// </summary>
        public VersionMessage()
        {
            Command = MessageCommand.version;
            Payload = new VersionPayload();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload">Payload</param>
        public VersionMessage(VersionPayload payload)
        {
            Command = MessageCommand.version;
            Payload = payload;
        }
    }

    public class VersionPayload
    {
        [BinaryProperty(0)]
        public uint Version;

        [BinaryProperty(1)]
        public ulong Services;

        [BinaryProperty(2)]
        public uint Timestamp;

        [BinaryProperty(3)]
        public ushort Port;

        [BinaryProperty(4)]
        public uint Nonce;

        [BinaryProperty(5, MaxLength = 255)]
        public string UserAgent;

        [BinaryProperty(6)]
        public uint CurrentBlockIndex;

        [BinaryProperty(7)]
        public bool Relay;
    }
}