using NeoSharp.BinarySerialization.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NeoSharp.BinarySerialization.Cache
{
    internal class BinarySerializerCache
    {
        /// <summary>
        /// Cache
        /// </summary>
        internal static readonly IDictionary<Type, BinarySerializerCache> Cache = new Dictionary<Type, BinarySerializerCache>();
        internal static readonly IDictionary<Type, TypeConverter> TypeConverterCache = new Dictionary<Type, TypeConverter>();

        /// <summary>
        /// Cache types (call me if you load a new plugin or module)
        /// </summary>
        /// <param name="asms">Assemblies</param>
        public static void CacheTypesOf(params Assembly[] asms)
        {
            foreach (Assembly asm in asms)
            {
                foreach (var t in asm.GetTypes().Where(t => typeof(TypeConverter).IsAssignableFrom(t)))
                    InternalCacheTypeConvertersOf(t);

                foreach (var t in asm.GetTypes().Where(t => typeof(TypeConverter).IsAssignableFrom(t) == false))
                    InternalCacheTypesOf(t);
            }
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

        internal static void InternalCacheTypeConvertersOf(Type type)
        {
            lock (TypeConverterCache)
            {
                if (TypeConverterCache.ContainsKey(type)) return;

                TypeConverterCache.Add(type, (TypeConverter)Activator.CreateInstance(type));
            }
        }

        /// <summary>
        /// Type
        /// </summary>
        public readonly Type Type;
        /// <summary>
        /// Count
        /// </summary>
        public readonly int Count;
        /// <summary>
        /// Is OnPreSerializable
        /// </summary>
        public readonly bool IsOnPreSerializable;
        /// <summary>
        /// Is OnPostDeserializable
        /// </summary>
        public readonly bool IsOnPostDeserializable;
        /// <summary>
        /// Cache entries
        /// </summary>
        private readonly BinarySerializerCacheEntry[] _entries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        public BinarySerializerCache(Type type)
        {
            Type = type;

            // Check interfaces

            IsOnPreSerializable = typeof(IBinaryOnPreSerializable).IsAssignableFrom(type);
            IsOnPostDeserializable = typeof(IBinaryOnPostDeserializable).IsAssignableFrom(type);

            _entries =

                // Properties

                type.GetProperties()
                .Select(u => new { prop = u, atr = u.GetCustomAttribute<BinaryPropertyAttribute>(true) })
                .Where(u => u.atr != null)
                .OrderBy(u => u.atr.Order)
                .Select(u => new BinarySerializerCacheEntry(u.atr, u.prop))
                .Concat
                (
                    // Fields

                    type.GetFields()
                    .Select(u => new { prop = u, atr = u.GetCustomAttribute<BinaryPropertyAttribute>(true) })
                    .Where(u => u.atr != null)
                    .OrderBy(u => u.atr.Order)
                    .Select(u => new BinarySerializerCacheEntry(u.atr, u.prop))
                )
                .ToArray();

            Count = _entries.Length;
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="bw">Stream</param>
        /// <param name="obj">Object</param>
        public int Serialize(IBinarySerializer serializer, BinaryWriter bw, object obj)
        {
            int ret = 0;
            foreach (BinarySerializerCacheEntry e in _entries)
                ret += e.WriteValue(serializer, bw, e.GetValue(obj));

            return ret;
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="deserializer">Deserializer</param>
        /// <param name="br">Stream</param>
        /// <returns>Return object</returns>
        public T Deserialize<T>(IBinaryDeserializer deserializer, BinaryReader br)
        {
            return (T)Deserialize(deserializer, br);
        }
        /// <summary>
        /// Deserialize without create a new object
        /// </summary>
        /// <param name="deserializer">Deserializer</param>
        /// <param name="br">Stream</param>
        /// <param name="obj">Object</param>
        public void DeserializeInside(IBinaryDeserializer deserializer, BinaryReader br, object obj)
        {
            foreach (BinarySerializerCacheEntry e in _entries)
            {
                if (e.ReadOnly)
                {
                    // Consume it
                    e.ReadValue(deserializer, br);
                    continue;
                }

                e.SetValue(obj, e.ReadValue(deserializer, br));
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="deserializer">Deserializer</param>
        /// <param name="br">Stream</param>
        /// <returns>Return object</returns>
        public object Deserialize(IBinaryDeserializer deserializer, BinaryReader br)
        {
            object ret = Activator.CreateInstance(Type);
            DeserializeInside(deserializer, br, ret);
            return ret;
        }
    }
}