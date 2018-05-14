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
        public BinaryConverter(params Assembly[] asms) : base()
        {
            BinarySerializerCache.RegisterTypes(asms);
        }

        #region Binds

        public T Deserialize<T>(byte[] data) where T : new() => Deserializer.Deserialize<T>(data);
        public object Deserialize(byte[] data, Type type) => Deserializer.Deserialize(data, type);
        public T Deserialize<T>(Stream stream) where T : new() => Deserializer.Deserialize<T>(stream);
        public T Deserialize<T>(BinaryReader stream) where T : new() => Deserializer.Deserialize<T>(stream);
        public object Deserialize(Stream stream, Type t) => Deserializer.Deserialize(stream, t);
        public object Deserialize(BinaryReader stream, Type t) => Deserializer.Deserialize(stream, t);
        public void DeserializeInside(byte[] buffer, object obj) => Deserializer.DeserializeInside(buffer, obj);
        public void DeserializeInside(Stream stream, object obj) => Deserializer.DeserializeInside(stream, obj);
        public byte[] Serialize(object obj) => Serializer.Serialize(obj);
        public int Serialize(object obj, Stream stream) => Serializer.Serialize(obj, stream);
        public int Serialize(object obj, BinaryWriter stream) => Serializer.Serialize(obj, stream);

        #endregion
    }
}