using System;
using System.IO;
using System.Text;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class BinaryStringSerializer : IBinaryCustomSerializable
    {
        /// <summary>
        /// Max length
        /// </summary>
        public readonly int MaxLength;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxLength">Max length</param>
        public BinaryStringSerializer(int maxLength)
        {
            MaxLength = maxLength;
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            var data = Encoding.UTF8.GetBytes((string)value);

            if (data.Length >= MaxLength)
                throw new FormatException(nameof(MaxLength));

            return writer.WriteVarBytes(data);
        }

        public object Deserialize(IBinarySerializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            return reader.ReadVarString(MaxLength);
        }
    }
}