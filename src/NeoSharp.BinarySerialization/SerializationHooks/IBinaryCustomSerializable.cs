using System;
using System.IO;

namespace NeoSharp.BinarySerialization.SerializationHooks
{
    public interface IBinaryCustomSerializable
    {
        /// <summary>
        /// Deserialize logic
        /// </summary>
        /// <param name="deserializer">Deserializer</param>
        /// <param name="reader">Reader</param>
        /// <param name="type">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Deserialized object</returns>
        object Deserialize(IBinarySerializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null);
     
        /// <summary>
        /// Serialize logic
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="writer">Writer</param>
        /// <param name="value">Value</param>
        /// <param name="settings">Settings</param>
        /// <returns>How many bytes have been written</returns>
        int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null);
    }
}