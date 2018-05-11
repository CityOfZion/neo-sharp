using NeoSharp.BinarySerialization.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NeoSharp.BinarySerialization
{
    public class BinarySerializer
    {
        /// <summary>
        /// Cache
        /// </summary>
        internal static readonly Dictionary<Type, BinarySerializerCache> Cache = new Dictionary<Type, BinarySerializerCache>();

        /// <summary>
        /// Cache Binary Serializer types
        /// </summary>
        static BinarySerializer()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                try
                {
                    // Speed up warm-up process
                    if (!asm.FullName.StartsWith("Neo")) continue;

                    CacheTypesOf(asm);
                }
                catch { }
        }

        /// <summary>
        /// Cache types (call me if you load a new plugin or module)
        /// </summary>
        /// <param name="asm">Assembly</param>
        public static void CacheTypesOf(Assembly asm)
        {
            foreach (var t in asm.GetTypes())
                CacheTypesOf(t);
        }
        /// <summary>
        /// Cache type
        /// </summary>
        /// <param name="type">Type</param>
        public static bool CacheTypesOf(Type type)
        {
            return InternalCacheTypesOf(type) != null;
        }
        /// <summary>
        /// Cache type
        /// </summary>
        /// <param name="type">Type</param>
        internal static BinarySerializerCache InternalCacheTypesOf(Type type)
        {
            lock (Cache)
            {
                if (Cache.TryGetValue(type, out var cache)) return cache;

                var b = new BinarySerializerCache(type);
                if (b.Count <= 0) return null;

                Cache.Add(b.Type, b);
                return b;
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        public static T Deserialize<T>(byte[] data) where T : new()
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
        public static object Deserialize(byte[] data, Type type)
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
        public static T Deserialize<T>(Stream stream) where T : new()
        {
            // Search in cache

            if (!Cache.TryGetValue(typeof(T), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8))
            {
                var obj = cache.Deserialize<T>(br);

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
        public static T Deserialize<T>(BinaryReader stream) where T : new()
        {
            // Search in cache

            if (!Cache.TryGetValue(typeof(T), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            var obj = cache.Deserialize<T>(stream);

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
        public static object Deserialize(Stream stream, Type t)
        {
            // Search in cache

            if (!Cache.TryGetValue(t, out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var obj = cache.Deserialize(br);

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
        public static void DeserializeInside(byte[] buffer, object obj)
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
        public static void DeserializeInside(Stream stream, object obj)
        {
            // Search in cache

            if (!Cache.TryGetValue(obj.GetType(), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                cache.DeserializeInside(br, obj);

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
        public static object Deserialize(BinaryReader stream, Type t)
        {
            // Search in cache

            if (!Cache.TryGetValue(t, out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            var obj = cache.Deserialize(stream);

            if (cache.IsOnPostDeserializable)
                ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();

            return obj;
        }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        public static byte[] Serialize(object obj)
        {
            using (var ms = new MemoryStream())
            {
                Serialize(obj, ms);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <returns>Return byte array</returns>
        public static int Serialize(object obj, Stream stream)
        {
            // Search in cache

            if (!Cache.TryGetValue(obj.GetType(), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Serialize

            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                if (cache.IsOnPreSerializable)
                    ((IBinaryOnPreSerializable)obj).OnPreSerialize();

                return cache.Serialize(bw, obj);
            }
        }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <returns>Return byte array</returns>
        public static int Serialize(object obj, BinaryWriter stream)
        {
            // Search in cache

            if (!Cache.TryGetValue(obj.GetType(), out var cache))
                throw new NotImplementedException();

            // Serialize

            if (cache.IsOnPreSerializable)
                ((IBinaryOnPreSerializable)obj).OnPreSerialize();

            return cache.Serialize(stream, obj);
        }
    }
}