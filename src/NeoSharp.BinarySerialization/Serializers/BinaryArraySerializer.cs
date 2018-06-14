using System;
using System.IO;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class BinaryArraySerializer : IBinaryCustomSerializable
    {
        private readonly Type Type;
        private readonly IBinaryCustomSerializable Serializer;

        /// <summary>
        /// Max length
        /// </summary>
        public readonly int MaxLength;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        /// <param name="maxLength">Max length</param>
        /// <param name="serializer">Serializer</param>
        public BinaryArraySerializer(Type type, int maxLength, IBinaryCustomSerializable serializer)
        {
            MaxLength = maxLength;
            Serializer = serializer;
            Type = type;
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            var ar = (Array)value;

            if (ar == null)
            {
                return writer.WriteVarInt(0);
            }

            var x = writer.WriteVarInt(ar.Length);

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

            var a = (Array)Activator.CreateInstance(Type, l);

            for (var ix = 0; ix < l; ix++)
            {
                a.SetValue(Serializer.Deserialize(deserializer, reader, type, settings), ix);
            }

            return a;
        }
    }
}