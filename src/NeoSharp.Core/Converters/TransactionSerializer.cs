using System;
using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.SerializationHooks;
using NeoSharp.Core.Caching;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Converters
{
    public class TransactionSerializer : IBinaryCustomSerializable
    {
        /// <summary>
        /// Cache
        /// </summary>
        readonly static ReflectionCache<byte> _cache = ReflectionCache<byte>.CreateFromEnum<TransactionType>();

        public object Deserialize(IBinaryDeserializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            // Read transaction Type

            var tx = _cache.CreateInstance<Transaction>((byte)reader.PeekChar());

            tx.Deserialize(deserializer, reader, settings);

            return tx;
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            var tx = (Transaction)value;
            return tx.Serialize(serializer, writer, settings);
        }
    }
}