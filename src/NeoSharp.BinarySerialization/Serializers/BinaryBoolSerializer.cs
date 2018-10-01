using System;
using System.IO;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class BinaryBoolSerializer : IBinaryCustomSerializable
    {
        private const byte BTRUE = 0x01;
        private const byte BFALSE = 0x00;

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            if ((bool)value) writer.Write(BTRUE);
            else writer.Write(BFALSE);

            return 1;
        }

        public object Deserialize(IBinarySerializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            return reader.ReadByte() != BFALSE;
        }
    }
}