using NeoSharp.BinarySerialization.Cache;
using NeoSharp.BinarySerialization.SerializationHooks;
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
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return byte array</returns>
        public T Deserialize<T>(byte[] data, BinarySerializerSettings settings = null) where T : new()
        {
            using (var ms = new MemoryStream(data))
            {
                return Deserialize<T>(ms, settings);
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="type">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        public object Deserialize(byte[] data, Type type, BinarySerializerSettings settings = null)
        {
            using (var ms = new MemoryStream(data))
            {
                return Deserialize(ms, type, settings);
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        public T Deserialize<T>(Stream stream, BinarySerializerSettings settings = null) where T : new()
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(typeof(T), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8))
            {
                var obj = cache.Deserialize<T>(this, br, settings);

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
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        public T Deserialize<T>(BinaryReader stream, BinarySerializerSettings settings = null) where T : new()
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(typeof(T), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            var obj = cache.Deserialize<T>(this, stream, settings);

            if (cache.IsOnPostDeserializable)
                ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();

            return obj;
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="t">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        public object Deserialize(Stream stream, Type t, BinarySerializerSettings settings = null)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(t, out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                var obj = cache.Deserialize(this, br, settings);

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
        /// <param name="settings">Settings</param>
        public void Deserialize(byte[] buffer, object obj, BinarySerializerSettings settings = null)
        {
            using (var ms = new MemoryStream(buffer))
            {
                Deserialize(ms, obj, settings);
            }
        }
        /// <summary>
        /// Deserialize without create a new object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="settings">Settings</param>
        public void Deserialize(Stream stream, object obj, BinarySerializerSettings settings = null)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(obj.GetType(), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                cache.Deserialize(this, br, obj, settings);

                if (cache.IsOnPostDeserializable)
                    ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="t">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        public object Deserialize(BinaryReader stream, Type t, BinarySerializerSettings settings = null)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(t, out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            var obj = cache.Deserialize(this, stream, settings);

            if (cache.IsOnPostDeserializable)
                ((IBinaryOnPostDeserializable)obj).OnPostDeserialize();

            return obj;
        }
    }
}