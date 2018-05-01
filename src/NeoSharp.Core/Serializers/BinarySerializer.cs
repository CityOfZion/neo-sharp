using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NeoSharp.Core.Serializers
{
    public class BinarySerializer
    {
        /// <summary>
        /// Cache
        /// </summary>
        readonly static Dictionary<Type, BinarySerializerCache> Cache = new Dictionary<Type, BinarySerializerCache>();

        /// <summary>
        /// Cache Binary Serializer types
        /// </summary>
        static BinarySerializer()
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
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
            foreach (Type t in asm.GetTypes())
                CacheTypesOf(t);
        }
        /// <summary>
        /// Cache type
        /// </summary>
        /// <param name="type">Type</param>
        public static bool CacheTypesOf(Type type)
        {
            lock (Cache)
            {
                if (Cache.TryGetValue(type, out BinarySerializerCache cache)) return false;

                BinarySerializerCache b = new BinarySerializerCache(type);
                if (b.Count <= 0) return false;

                Cache.Add(b.Type, b);
            }

            return true;
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        public static T Deserialize<T>(byte[] data) where T : new()
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Deserialize<T>(ms);
            }
        }
        public static object Deserialize(byte[] data, Type type)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Deserialize(ms, type);
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        public static T Deserialize<T>(Stream data) where T : new()
        {
            // Search in cache

            Type t = typeof(T);
            if (!Cache.TryGetValue(t, out BinarySerializerCache cache))
                throw (new NotImplementedException());

            // Deserialize

            using (BinaryReader br = new BinaryReader(data, Encoding.UTF8))
            {
                T obj = cache.Deserialize<T>(br);

                if (cache.IsOnPostDeserializable)
                    ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();

                return obj;
            }
        }
        public static object Deserialize(Stream data, Type t)
        {
            // Search in cache

            if (!Cache.TryGetValue(t, out BinarySerializerCache cache))
                throw (new NotImplementedException());

            // Deserialize

            using (BinaryReader br = new BinaryReader(data, Encoding.UTF8))
            {
                var obj = cache.Deserialize(br);

                if (cache.IsOnPostDeserializable)
                    ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();

                return obj;
            }
        }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        public static byte[] Serialize<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Serialize(obj, ms);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <returns>Return byte array</returns>
        public static int Serialize<T>(T obj, Stream stream)
        {
            // Search in cache

            if (!Cache.TryGetValue(typeof(T), out BinarySerializerCache cache))
                throw (new NotImplementedException());

            // Serialize

            using (BinaryWriter bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                if (cache.IsOnPreSerializable)
                    ((IBinaryOnPreSerializable)obj).OnPreSerialize();

                return cache.Serialize(bw, obj);
            }
        }
    }
}