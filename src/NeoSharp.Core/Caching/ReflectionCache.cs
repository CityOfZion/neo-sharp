using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NeoSharp.Core.Caching
{
    public class ReflectionCache<T> : Dictionary<T, Type>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <typeparam name="TEnumType">Enum type</typeparam>
        public static ReflectionCache<T> CreateFromEnum<TEnumType>() where TEnumType : struct, IConvertible
        {
            var enumType = typeof(TEnumType);

            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException("K must be an enumerated type");

            // Cache all types
            var r = new ReflectionCache<T>();

            foreach (var t in Enum.GetValues(enumType))
            {
                // Get enumn member
                var memInfo = enumType.GetMember(t.ToString());
                if (memInfo == null || memInfo.Length != 1)
                    throw (new FormatException());

                // Get attribute
                var attribute = memInfo[0].GetCustomAttributes(typeof(ReflectionCacheAttribute), false)
                    .Cast<ReflectionCacheAttribute>()
                    .FirstOrDefault();

                if (attribute == null)
                    continue;

                // Append to cache
                r.Add((T)t, attribute.Type);
            }
            return r;
        }
        /// <summary>
        /// Create object from key
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="def">Default value</param>
        public object CreateInstance(T key, object def = null)
        {
            // Get Type from cache
            if (TryGetValue(key, out var tp)) return Activator.CreateInstance(tp);

            // return null
            return def;
        }
        /// <summary>
        /// Create object from key
        /// </summary>
        /// <typeparam name="TK">Type</typeparam>
        /// <param name="key">Key</param>
        /// <param name="def">Default value</param>
        public TK CreateInstance<TK>(T key, TK def = default(TK))
        {
            // Get Type from cache
            if (TryGetValue(key, out var tp)) return (TK)Activator.CreateInstance(tp);

            // return null
            return def;
        }
    }
}