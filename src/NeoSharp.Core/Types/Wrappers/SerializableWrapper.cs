using System;
using System.IO;

namespace NeoSharp.Core.Types.Wrappers
{
    public abstract class SerializableWrapper : ISerializable
    {
        public abstract int Size { get; }

        public abstract void Deserialize(BinaryReader reader);

        public abstract void Serialize(BinaryWriter writer);

        public static implicit operator SerializableWrapper(byte value)
        {
            return new ByteWrapper(value);
        }
    }

    public abstract class SerializableWrapper<T> : SerializableWrapper where T : IEquatable<T>
    {
    }
}
