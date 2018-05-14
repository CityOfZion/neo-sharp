using NeoSharp.BinarySerialization.Cache;
using NeoSharp.BinarySerialization.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NeoSharp.BinarySerialization
{
    public class BinaryDeserializer : IBinaryDeserializer
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BinaryDeserializer() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="asms">Assemblies</param>
        public BinaryDeserializer(params Assembly[] asms)
        {
            BinarySerializerCache.CacheTypesOf(asms);
        }

        /// <summary>
        /// Cache types (call me if you load a new plugin or module)
        /// </summary>
        /// <param name="asms">Assemblies</param>
        public static void CacheTypesOf(params Assembly[] asms)
        {
            BinarySerializerCache.CacheTypesOf(asms);
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        public T Deserialize<T>(byte[] data) where T : new()
        {
            using (var ms = new MemoryStream(data))
            {
                return Deserialize<T>(ms);
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="type">Type</param>
        /// <returns>Return object</returns>
        public object Deserialize(byte[] data, Type type)
        {
            using (var ms = new MemoryStream(data))
            {
                return Deserialize(ms, type);
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stream">Stream</param>
        /// <returns>Return object</returns>
        public T Deserialize<T>(Stream stream) where T : new()
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(typeof(T), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8))
            {
                var obj = cache.Deserialize<T>(this, br);

                if (cache.IsOnPostDeserializable)
                    ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();

                return obj;
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stream">Stream</param>
        /// <returns>Return object</returns>
        public T Deserialize<T>(BinaryReader stream) where T : new()
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(typeof(T), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            var obj = cache.Deserialize<T>(this, stream);

            if (cache.IsOnPostDeserializable)
                ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();

            return obj;
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="t">Type</param>
        /// <returns>Return object</returns>
        public object Deserialize(Stream stream, Type t)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(t, out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var obj = cache.Deserialize(this, br);

                if (cache.IsOnPostDeserializable)
                    ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();

                return obj;
            }
        }
        /// <summary>
        /// Deserialize without create a new object
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="obj">Object</param>
        public void DeserializeInside(byte[] buffer, object obj)
        {
            using (var ms = new MemoryStream(buffer))
            {
                DeserializeInside(ms, obj);
            }
        }
        /// <summary>
        /// Deserialize without create a new object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        public void DeserializeInside(Stream stream, object obj)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(obj.GetType(), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                cache.DeserializeInside(this, br, obj);

                if (cache.IsOnPostDeserializable)
                    ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="t">Type</param>
        /// <returns>Return object</returns>
        public object Deserialize(BinaryReader stream, Type t)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(t, out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            var obj = cache.Deserialize(this, stream);

            if (cache.IsOnPostDeserializable)
                ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();

            return obj;
        }
    }
}