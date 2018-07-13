using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NeoSharp.BinarySerialization.Cache;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.BinarySerialization.Serializers
{
    internal class BinaryAutoSerializer : IBinaryCustomSerializable
    {
        /// <summary>
        /// Type
        /// </summary>
        public readonly Type Type;
        /// <summary>
        /// IsEmpty
        /// </summary>
        public bool IsEmpty => _entries.Length == 0;

        /// <summary>
        /// Cache entries
        /// </summary>
        private readonly BinarySerializerCacheEntry[] _entries;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        public BinaryAutoSerializer(Type type)
        {
            Type = type;

            _entries =

            // Properties

            type.GetProperties()
            .Select(u => new { prop = u, atr = u.GetCustomAttribute<BinaryPropertyAttribute>(true) })
            .Where(u => u.atr != null)
            .Select(u => new BinarySerializerCacheEntry(u.atr, u.prop))
            .Concat
            (
                // Fields

                type.GetFields()
                .Select(u => new { prop = u, atr = u.GetCustomAttribute<BinaryPropertyAttribute>(true) })
                .Where(u => u.atr != null)
                .Select(u => new BinarySerializerCacheEntry(u.atr, u.prop))
            )
            .OrderBy(u => u.Order)
            .GroupBy(u => u.Order, (a, b) => b.OrderByDescending(u => u.Override).FirstOrDefault())
            .ToArray();
        }

        /// <summary>
        /// Serialize
        /// </summary>
        /// <param name="serializer">Serializer</param>
        /// <param name="bw">Stream</param>
        /// <param name="obj">Object</param>
        /// <param name="settings">Settings</param>
        public int Serialize(IBinarySerializer serializer, BinaryWriter bw, object obj, BinarySerializerSettings settings = null)
        {
            if (settings != null && settings.Filter != null)
            {
                return _entries.Where(e => settings.Filter.Invoke(e.Name) != false)
                           .Sum(e => e.Serializer.Serialize(serializer, bw, e.GetValue(obj)));
            }
            else
            {
                return _entries.Sum(e => e.Serializer.Serialize(serializer, bw, e.GetValue(obj)));
            }
        }
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="deserializer">Deserializer</param>
        /// <param name="reader">Reader</param>
        /// <param name="settings">Settings</param>
        /// <returns>Deserialized object</returns>
        public T Deserialize<T>(IBinaryDeserializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            return (T)Deserialize(deserializer, reader, Type, settings);
        }
        /// <summary>
        /// Deserialize object
        /// </summary>
        /// <param name="deserializer">Deserializer</param>
        /// <param name="reader">Reader</param>
        /// <param name="settings">Settings</param>
        /// <returns>Deserialized object</returns>
        public object Deserialize(IBinaryDeserializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
        {
            return Deserialize(deserializer, reader, Type, settings);
        }

        /// <summary>
        /// Deserialize object
        /// </summary>
        /// <param name="deserializer">Deserializer</param>
        /// <param name="reader">Reader</param>
        /// <param name="type">Type</param>
        /// <param name="settings">Settings</param>
        /// <returns>Deserialized object</returns>
        public object Deserialize(IBinaryDeserializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            var ret = Activator.CreateInstance(type);

            foreach (var e in _entries)
            {
                if (settings?.Filter?.Invoke(e.Name) == false) continue;

                if (e.ReadOnly)
                {
                    // Consume it

                    var val = e.Serializer.Deserialize(deserializer, reader, e.Type, settings);

                    // Should be equal

                    if (!val.Equals(e.GetValue(ret)))
                    {
                        // If a readonly property or field is not the same, throw and exception !

                        throw new FormatException();
                    }

                    continue;
                }

                e.SetValue(ret, e.Serializer.Deserialize(deserializer, reader, e.Type, settings));
            }

            if (ret is IBinaryVerifiable v && !v.Verify())
            {
                throw new FormatException();
            }

            return ret;
        }
    }
}