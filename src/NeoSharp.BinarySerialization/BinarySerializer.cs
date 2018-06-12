using NeoSharp.BinarySerialization.Cache;
using NeoSharp.BinarySerialization.SerializationHooks;
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
        /// Constructor
        /// </summary>
        public BinarySerializer() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="asms">Assemblies</param>
        public BinarySerializer(params Assembly[] asms)
        {
            BinarySerializerCache.RegisterTypes(asms);
        }

        /// <summary>
        /// Register types (call me if you load a new plugin or module)
        /// </summary>
        /// <param name="asms">Assemblies</param>
        public static void RegisterTypes(params Assembly[] asms)
        {
            BinarySerializerCache.RegisterTypes(asms);
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return byte array</returns>
        public byte[] Serialize(object obj, BinarySerializerSettings settings = null)
        {
            using (var ms = new MemoryStream())
            {
                Serialize(obj, ms, settings);
                return ms.ToArray();
            }
        }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return byte array</returns>
        public int Serialize(object obj, Stream stream, BinarySerializerSettings settings = null)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(obj.GetType(), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Serialize

            using (var bw = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                if (cache.IsOnPreSerializable)
                    ((IBinaryOnPreSerializable)obj).OnPreSerialize();

                return cache.Serialize(this, bw, obj, settings);
            }
        }
        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="stream">Stream</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return byte array</returns>
        public int Serialize(object obj, BinaryWriter stream, BinarySerializerSettings settings = null)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(obj.GetType(), out var cache))
                throw new NotImplementedException();

            // Serialize

            if (cache.IsOnPreSerializable)
                ((IBinaryOnPreSerializable)obj).OnPreSerialize();

            return cache.Serialize(this, stream, obj, settings);
        }
    }
}