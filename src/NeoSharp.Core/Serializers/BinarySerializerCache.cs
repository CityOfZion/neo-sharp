using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NeoSharp.Core.Serializers
{
    internal class BinarySerializerCache
    {
        /// <summary>
        /// Type
        /// </summary>
        public readonly Type Type;
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

            Entries =
                type.GetProperties()
                .Select(u => new { prop = u, atr = u.GetCustomAttribute<BinaryPropertyAttribute>() })
                .Where(u => u.atr != null)
                .OrderBy(u => u.atr.Order)
                .Select(u => new BinarySerializerCacheEntry(u.prop))
                .ToArray();
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
                ret += e.SetValue(bw, e.Property.GetValue(obj));

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
        /// Deserialize
        /// </summary>
        /// <param name="br">Stream</param>
        /// <returns>Return object</returns>
        public object Deserialize(BinaryReader br)
        {
            object ret = Activator.CreateInstance(Type);

            foreach (BinarySerializerCacheEntry e in Entries)
            {
                e.Property.SetValue(ret, e.GetValue(br));
            }

            return ret;
        }
    }
}