using System;
using System.Collections.Generic;
using System.IO;
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
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        public static T Deserialize<T>(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                return Deserialize<T>(ms);
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        public static T Deserialize<T>(Stream data)
        {
            // Search in cache

            Type t = typeof(T);
            if (!Cache.TryGetValue(t, out BinarySerializerCache cache))
            {
                cache = new BinarySerializerCache(t);
                Cache[t] = cache;
            }

            // Deserialize

            using (BinaryReader br = new BinaryReader(data, Encoding.UTF8))
            {
                return cache.Deserialize<T>(br);
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

            Type t = typeof(T);
            if (!Cache.TryGetValue(t, out BinarySerializerCache cache))
            {
                cache = new BinarySerializerCache(t);
                Cache[t] = cache;
            }

            // Serialize

            using (BinaryWriter bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                return cache.Serialize(bw, obj);
            }
        }
    }
}