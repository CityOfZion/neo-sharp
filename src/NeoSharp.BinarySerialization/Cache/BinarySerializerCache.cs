using System;
using System.Collections.Generic;
using System.Reflection;
using NeoSharp.BinarySerialization.SerializationHooks;
using NeoSharp.BinarySerialization.Serializers;

namespace NeoSharp.BinarySerialization.Cache
{
    internal class BinarySerializerCache
    {
        #region Cache

        /// <summary>
        /// Cache
        /// </summary>
        internal static readonly IDictionary<Type, IBinaryCustomSerializable> Cache = new Dictionary<Type, IBinaryCustomSerializable>();

        /// <summary>
        /// Cache types (call me if you load a new plugin or module)
        /// </summary>
        /// <param name="asms">Assemblies</param>
        public static void RegisterTypes(params Assembly[] asms)
        {
            foreach (var asm in asms)
            {
                RegisterTypes(asm.GetTypes());
            }
        }

        /// <summary>
        /// Cache types (call me if you load a new plugin or module)
        /// </summary>
        /// <param name="types">Types</param>
        public static void RegisterTypes(params Type[] types)
        {
            foreach (var t in types)
            {
                InternalRegisterTypes(t);
            }
        }

        /// <summary>
        /// Cache type
        /// </summary>
        /// <param name="type">Type</param>
        internal static IBinaryCustomSerializable InternalRegisterTypes(Type type)
        {
            lock (Cache)
            {
                if (Cache.TryGetValue(type, out var cache)) return cache;

                IBinaryCustomSerializable serializer;
                var serializerAttr = type.GetCustomAttribute<BinaryTypeSerializerAttribute>();

                if (serializerAttr != null)
                {
                    // Looking for a serializer

                    serializer = serializerAttr.Create();
                }
                else
                {
                    // Create one by his fields and properties

                    serializer = new BinaryAutoSerializer(type);

                    if (((BinaryAutoSerializer)serializer).IsEmpty) return null;
                }

                Cache.Add(type, serializer);

                if (!type.IsArray)
                {
                    // Register array too

                    var array = type.MakeArrayType();
                    Cache.Add(array, new BinaryArraySerializer(array, serializer));
                }

                return serializer;
            }
        }

        #endregion
    }
}
