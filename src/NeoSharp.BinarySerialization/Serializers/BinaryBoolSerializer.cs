using System;
using System.IO;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    public class BinaryBoolSerializer : IBinaryCustomSerialization
    {
        private const byte BTRUE = 0x01;
        private const byte BFALSE = 0x00;

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            if ((bool)value) writer.Write(BTRUE);
            else writer.Write(BFALSE);

            return 1;
        }

        public object Deserialize(IBinaryDeserializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            return reader.ReadByte() != BFALSE;
        }
    }
}