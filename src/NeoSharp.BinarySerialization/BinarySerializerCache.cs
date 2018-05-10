using NeoSharp.BinarySerialization.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NeoSharp.BinarySerialization
{
    internal class BinarySerializerCache
    {
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
        readonly BinarySerializerCacheEntry[] Entries;

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

            Entries =

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

            Count = Entries.Length;
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="bw">Stream</param>
        /// <param name="obj">Object</param>
        public int Serialize(BinaryWriter bw, object obj)
        {
            int ret = 0;
            foreach (BinarySerializerCacheEntry e in Entries)
                ret += e.WriteValue(bw, e.GetValue(obj));

            return ret;
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="br">Stream</param>
        /// <returns>Return object</returns>
        public T Deserialize<T>(BinaryReader br)
        {
            return (T)Deserialize(br);
        }
        /// <summary>
        /// Deserialize without create a new object
        /// </summary>
        /// <param name="br">Stream</param>
        /// <param name="obj">Object</param>
        public void DeserializeInside(BinaryReader br, object obj)
        {
            foreach (BinarySerializerCacheEntry e in Entries)
            {
                if (e.ReadOnly)
                {
                    // Consume it
                    e.ReadValue(br);
                    continue;
                }

                e.SetValue(obj, e.ReadValue(br));
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="br">Stream</param>
        /// <returns>Return object</returns>
        public object Deserialize(BinaryReader br)
        {
            object ret = Activator.CreateInstance(Type);
            DeserializeInside(br, ret);
            return ret;
        }
    }
}