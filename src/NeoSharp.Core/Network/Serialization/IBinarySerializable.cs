using System.IO;

namespace NeoSharp.Core.Network.Serialization
{
    public interface IBinarySerializable
    {
        void Serialize(BinaryWriter writer);
        void Deserialize(BinaryReader reader);
    }
}