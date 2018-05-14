using NeoSharp.BinarySerialization.Cache;
using NeoSharp.BinarySerialization.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NeoSharp.BinarySerialization
{
    public class BinarySerializer : IBinarySerializer
    {
        /// <summary>
        /// Cache types (call me if you load a new plugin or module)
        /// </summary>
        /// <param name="asm">Assembly</param>
        public static void CacheTypesOf(Assembly asm)
        {
            BinarySerializerCache.CacheTypesOf(asm);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <returns>Return byte array</returns>
        public byte[] Serialize(object obj)
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
        public int Serialize(object obj, Stream stream)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(obj.GetType(), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Serialize

            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                if (cache.IsOnPreSerializable)
                    ((IBinaryOnPreSerializable)obj).OnPreSerialize();

                return cache.Serialize(this, bw, obj);
            }
        }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <returns>Return byte array</returns>
        public int Serialize(object obj, BinaryWriter stream)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(obj.GetType(), out var cache))
                throw new NotImplementedException();

            // Serialize

            if (cache.IsOnPreSerializable)
                ((IBinaryOnPreSerializable)obj).OnPreSerialize();

            return cache.Serialize(this, stream, obj);
        }
    }
}