using System;
using System.IO;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class BinaryByteArraySerializer : IBinaryCustomSerializable
    {
        /// <summary>
        /// Max length
        /// </summary>
        public readonly int MaxLength;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxLength">Max length</param>
        public BinaryByteArraySerializer(int maxLength)
        {
            MaxLength = maxLength;
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            var ar = (byte[])value;

            if (ar == null)
                return writer.WriteVarInt(0);

            if (ar.Length > MaxLength)
                throw new FormatException(nameof(MaxLength));

            return writer.WriteVarBytes(ar);
        }

        public object Deserialize(IBinarySerializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            return reader.ReadVarBytes(MaxLength);
        }
    }
}