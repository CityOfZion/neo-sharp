using NeoSharp.BinarySerialization.Cache;
using System;
using System.IO;
using System.Reflection;

namespace NeoSharp.BinarySerialization
{
    public class BinaryConverter : IBinaryConverter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="asms">Assemblies</param>
        public BinaryConverter(params Assembly[] asms)
        {
            BinarySerializerCache.RegisterTypes(asms);
        }

        #region Binds

        public object Deserialize(byte[] data, Type type, BinarySerializerSettings settings = null) => BinaryDeserializer.Default.Deserialize(data, type, settings);
        public object Deserialize(Stream stream, Type type, BinarySerializerSettings settings = null) => BinaryDeserializer.Default.Deserialize(stream, type, settings);
        public object Deserialize(BinaryReader stream, Type type, BinarySerializerSettings settings = null) => BinaryDeserializer.Default.Deserialize(stream, type, settings);

        public T Deserialize<T>(byte[] data, BinarySerializerSettings settings = null) => BinaryDeserializer.Default.Deserialize<T>(data, settings);
        public T Deserialize<T>(Stream stream, BinarySerializerSettings settings = null) => BinaryDeserializer.Default.Deserialize<T>(stream, settings);
        public T Deserialize<T>(BinaryReader stream, BinarySerializerSettings settings = null) => BinaryDeserializer.Default.Deserialize<T>(stream, settings);

        public byte[] Serialize(object obj, BinarySerializerSettings settings = null) => BinarySerializer.Default.Serialize(obj, settings);
        public int Serialize(object obj, Stream stream, BinarySerializerSettings settings = null) => BinarySerializer.Default.Serialize(obj, stream, settings);
        public int Serialize(object obj, BinaryWriter stream, BinarySerializerSettings settings = null) => BinarySerializer.Default.Serialize(obj, stream, settings);

        #endregion
    }
}
