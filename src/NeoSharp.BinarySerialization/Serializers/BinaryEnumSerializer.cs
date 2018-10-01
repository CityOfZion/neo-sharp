using System;
using System.IO;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class BinaryEnumSerializer : IBinaryCustomSerializable
    {
        private readonly Type Type;
        private readonly IBinaryCustomSerializable Serializer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeArray">Type array</param>
        /// <param name="serializer">Serializer</param>
        public BinaryEnumSerializer(Type typeEnum, IBinaryCustomSerializable serializer)
        {
            Type = typeEnum;
            Serializer = serializer;
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            return Serializer.Serialize(serializer, writer, value, settings);
        }

        public object Deserialize(IBinarySerializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            var ret = Serializer.Deserialize(deserializer, reader, type, settings);

            // TODO: Check if values outside the enum throw an exception (should throw it)

            return Enum.ToObject(Type, ret);
        }
    }
}