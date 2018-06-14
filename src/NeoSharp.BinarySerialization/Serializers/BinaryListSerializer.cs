using System;
using System.Collections;
using System.IO;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class BinaryListSerializer : IBinaryCustomSerializable
    {
        private readonly Type Type, ItemType;
        private readonly IBinaryCustomSerializable Serializer;

        /// <summary>
        /// Max length
        /// </summary>
        public readonly int MaxLength;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="typeList">Type list</param>
        /// <param name="serializer">Serializer</param>
        /// <param name="maxLength">Max length</param>
        public BinaryListSerializer(Type typeList, IBinaryCustomSerializable serializer, int maxLength = ushort.MaxValue)
        {
            Type = typeList;
            ItemType = typeList.GetElementType();
            Serializer = serializer;
            MaxLength = maxLength;
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            var ar = (IList)value;

            if (ar == null)
            {
                return writer.WriteVarInt(0);
            }

            var x = writer.WriteVarInt(ar.Count);

            if (x > MaxLength) throw new FormatException("MaxLength");

            foreach (var o in ar)
            {
                x += Serializer.Serialize(serializer, writer, o, settings);
            }

            return x;
        }

        public object Deserialize(IBinaryDeserializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            var l = (int)reader.ReadVarInt(ushort.MaxValue);
            if (l > MaxLength) throw new FormatException("MaxLength");

            var a = (IList)Activator.CreateInstance(Type);

            for (var ix = 0; ix < l; ix++)
            {
                a.Add(Serializer.Deserialize(deserializer, reader, ItemType, settings));
            }

            return a;
        }
    }
}