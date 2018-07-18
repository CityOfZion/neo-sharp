using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using NeoSharp.BinarySerialization.Cache;

namespace NeoSharp.BinarySerialization
{
    public class BinaryDeserializer : IBinaryDeserializer
    {
        public static IBinaryDeserializer Default { get; private set; } = new BinaryDeserializer();

        //Use BinaryInitializer to inject IBinaryDeserializer
        public static void Initialize(IBinaryDeserializer binaryDeserializer)
        {
            Default = binaryDeserializer;
        }

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
        public T Deserialize<T>(byte[] data, BinarySerializerSettings settings = null)
        {
            using (var ms = new MemoryStream(data))
            {
                return Deserialize<T>(ms, settings);
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        public T Deserialize<T>(Stream stream, BinarySerializerSettings settings = null)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(typeof(T), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                return (T)cache.Deserialize(this, br, typeof(T), settings);
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="stream">Stream</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        public T Deserialize<T>(BinaryReader stream, BinarySerializerSettings settings = null)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(typeof(T), out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            return (T)cache.Deserialize(this, stream, typeof(T), settings);
        }
        /// <summary>
        /// Deserialize without create a new object
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <param name="type">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Object</returns>
        public object Deserialize(byte[] buffer, Type type, BinarySerializerSettings settings = null)
        {
            using (var ms = new MemoryStream(buffer))
            {
                return Deserialize(ms, type, settings);
            }
        }
        /// <summary>
        /// Deserialize without create a new object
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Object</returns>
        public object Deserialize(Stream stream, Type type, BinarySerializerSettings settings = null)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(type, out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            using (var br = new BinaryReader(stream, Encoding.UTF8, true))
            {
                return cache.Deserialize(this, br, type, settings);
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="stream">Stream</param>
        /// <param name="type">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Return object</returns>
        public object Deserialize(BinaryReader stream, Type type, BinarySerializerSettings settings = null)
        {
            // Search in cache

            if (!BinarySerializerCache.Cache.TryGetValue(type, out var cache))
                throw new KeyNotFoundException("The type is not registered");

            // Deserialize

            return cache.Deserialize(this, stream, type, settings);
        }
    }
}