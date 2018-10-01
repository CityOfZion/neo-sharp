using System;
using System.Collections.Generic;
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
        /// Constructor indexes
        /// </summary>
        private readonly BinarySerializerCacheEntry[] _constructorIndexes;

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

            if (IsEmpty) return;

            foreach
                (
                var constructor in type
                .GetConstructors()
                .OrderBy(u => u.GetParameters().Length)
                )
            {
                var pars = constructor.GetParameters();

                if (pars == null || pars.Length == 0)
                {
                    // Don't search more, use public and without params
                    break;
                }

                var ix = 0;
                _constructorIndexes = new BinarySerializerCacheEntry[pars.Length];

                foreach (var par in pars)
                {
                    // The parameter should be named equal to the property

                    _constructorIndexes[ix] = _entries
                        .Where(u => u.Name.Equals(par.Name, StringComparison.InvariantCultureIgnoreCase))
                        .Single();

                    ix++;
                }
            }
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
        public T Deserialize<T>(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
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
        public object Deserialize(IBinarySerializer deserializer, BinaryReader reader, BinarySerializerSettings settings = null)
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
        public object Deserialize(IBinarySerializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            object ret;

            if (_constructorIndexes != null)
            {
                // Cache properties

                var dic = new Dictionary<BinarySerializerCacheEntry, object>();

                foreach (var e in _entries)
                {
                    if (settings?.Filter?.Invoke(e.Name) == false) continue;

                    dic.Add(e, e.Serializer.Deserialize(deserializer, reader, e.Type, settings));
                }

                // Call constructor

                var args = new object[_constructorIndexes.Length];

                for (var x = args.Length - 1; x >= 0; x--)
                {
                    // Don't set again the property

                    if (dic.Remove(_constructorIndexes[x], out var value))
                    {
                        args[x] = value;
                    }
                }

                // Create instance

                ret = Activator.CreateInstance(type, args);

                // Set other properties

                foreach (var e in dic)
                {
                    switch (e.Key.ValueHandlerLogic)
                    {
                        case ValueHandlerLogicType.Writable:
                            {
                                e.Key.SetValue(ret, e.Value);
                                break;
                            }
                        case ValueHandlerLogicType.JustConsume: break;
                        case ValueHandlerLogicType.MustBeEqual:
                            {
                                if (!e.Value.Equals(e.Key.GetValue(ret)))
                                {
                                    // If a readonly property or field is not the same, throw and exception !

                                    throw new FormatException(e.Key.Name);
                                }

                                break;
                            }
                    }
                }
            }
            else
            {
                // Faster way

                ret = Activator.CreateInstance(type);

                foreach (var e in _entries)
                {
                    if (settings?.Filter?.Invoke(e.Name) == false) continue;

                    switch (e.ValueHandlerLogic)
                    {
                        case ValueHandlerLogicType.Writable:
                            {
                                e.SetValue(ret, e.Serializer.Deserialize(deserializer, reader, e.Type, settings));
                                break;
                            }
                        case ValueHandlerLogicType.JustConsume:
                            {
                                // Consume it
                                e.Serializer.Deserialize(deserializer, reader, e.Type, settings);
                                break;
                            }
                        case ValueHandlerLogicType.MustBeEqual:
                            {
                                // Consume it

                                var val = e.Serializer.Deserialize(deserializer, reader, e.Type, settings);

                                // Should be equal

                                if (!val.Equals(e.GetValue(ret)))
                                {
                                    // If a readonly property or field is not the same, throw and exception !

                                    throw new FormatException(e.Name);
                                }

                                break;
                            }
                    }
                }
            }

            if (ret is IBinaryVerifiable v && !v.Verify())
            {
                throw new FormatException();
            }

            return ret;
        }
    }
}