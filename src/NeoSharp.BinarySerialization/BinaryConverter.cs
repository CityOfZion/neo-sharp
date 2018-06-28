using NeoSharp.BinarySerialization.Cache;
using System;
using System.IO;
using System.Reflection;

namespace NeoSharp.BinarySerialization
{
    public class BinaryConverter : IBinaryConverter
    {
        public readonly IBinarySerializer Serializer;
        public readonly IBinaryDeserializer Deserializer;

        /// <summary>
        /// Constructor
        /// </summary>
        public BinaryConverter()
        {
            Serializer = new BinarySerializer();
            Deserializer = new BinaryDeserializer();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="asms">Assemblies</param>
        public BinaryConverter(params Assembly[] asms) : this()
        {
            BinarySerializerCache.RegisterTypes(asms);
        }

        #region Binds

        public object Deserialize(byte[] data, Type type, BinarySerializerSettings settings = null) => Deserializer.Deserialize(data, type, settings);
        public object Deserialize(Stream stream, Type type, BinarySerializerSettings settings = null) => Deserializer.Deserialize(stream, type, settings);
        public object Deserialize(BinaryReader stream, Type type, BinarySerializerSettings settings = null) => Deserializer.Deserialize(stream, type, settings);

        public T Deserialize<T>(byte[] data, BinarySerializerSettings settings = null) => Deserializer.Deserialize<T>(data, settings);
        public T Deserialize<T>(Stream stream, BinarySerializerSettings settings = null) => Deserializer.Deserialize<T>(stream, settings);
        public T Deserialize<T>(BinaryReader stream, BinarySerializerSettings settings = null) => Deserializer.Deserialize<T>(stream, settings);

        public byte[] Serialize(object obj, BinarySerializerSettings settings = null) => Serializer.Serialize(obj, settings);
        public int Serialize(object obj, Stream stream, BinarySerializerSettings settings = null) => Serializer.Serialize(obj, stream, settings);
        public int Serialize(object obj, BinaryWriter stream, BinarySerializerSettings settings = null) => Serializer.Serialize(obj, stream, settings);

        #endregion
    }
}