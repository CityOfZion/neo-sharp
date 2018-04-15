using System.IO;

namespace NeoSharp.Core.Types
{
    /// <summary>
    /// Provides an interface for serialization
    /// </summary>
    public interface ISerializable
    {
        int Size { get; }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="writer">Writer to store serialized results</param>
        void Serialize(BinaryWriter writer);

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="reader">Reader to get deserialized value from</param>
        void Deserialize(BinaryReader reader);
    }
}
