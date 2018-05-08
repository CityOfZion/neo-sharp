using NeoSharp.BinarySerialization;

namespace NeoSharp.Core.Messaging.Messages
{
    public class VersionMessage : Message<VersionPayload>
    {
        public VersionMessage()
        {
            Command = MessageCommand.version;
            Payload = new VersionPayload();
        }
    }

    public class VersionPayload
    {
        [BinaryProperty(1)]
        public uint Version;

        [BinaryProperty(2)]
        public ulong Services;

        [BinaryProperty(3)]
        public uint Timestamp;

        [BinaryProperty(4)]
        public ushort Port;

        [BinaryProperty(5)]
        public uint Nonce;

        [BinaryProperty(6, MaxLength = 255)]
        public string UserAgent;

        [BinaryProperty(7)]
        public uint StartHeight;

        [BinaryProperty(8)]
        public bool Relay;
    }
}