using System.IO;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Network.Serialization;

namespace NeoSharp.Core.Network.Messages
{
    public class VersionMessage : Message<VersionPayload>
    {
        public VersionMessage()
        {
            Command = "version";
        }
    }

    public class VersionPayload : IBinarySerializable
    {
        public uint Version;
        public ulong Services;
        public uint Timestamp;
        public ushort Port;
        public uint Nonce;
        public string UserAgent;
        public uint StartHeight;
        public bool Relay;

        void IBinarySerializable.Serialize(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(Services);
            writer.Write(Timestamp);
            writer.Write(Port);
            writer.Write(Nonce);
            writer.WriteVarString(UserAgent);
            writer.Write(StartHeight);
            writer.Write(Relay);
        }

        void IBinarySerializable.Deserialize(BinaryReader reader)
        {
            Version = reader.ReadUInt32();
            Services = reader.ReadUInt64();
            Timestamp = reader.ReadUInt32();
            Port = reader.ReadUInt16();
            Nonce = reader.ReadUInt32();
            UserAgent = reader.ReadVarString(1024);
            StartHeight = reader.ReadUInt32();
            Relay = reader.ReadBoolean();
        }
    }
}