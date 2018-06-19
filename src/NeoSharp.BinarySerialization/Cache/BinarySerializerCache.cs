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
            foreach (Assembly asm in asms)
            {
                foreach (var t in asm.GetTypes())
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

                var b = new BinaryAutoSerializer(type);
                if (b.IsEmpty) return null;

                Cache.Add(b.Type, b);

                if (!b.Type.IsArray)
                {
                    // Register array too

                    Type array = b.Type.MakeArrayType();
                    Cache.Add(array, new BinaryAutoSerializer(array, new BinaryArraySerializer(array, b)));
                }
                return b;
            }
        }

        #endregion
    }
}