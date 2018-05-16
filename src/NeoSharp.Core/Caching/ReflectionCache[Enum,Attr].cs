using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NeoSharp.Core.Caching
{
    public class ReflectionCache<TEnumType, TAttrType> : Dictionary<TEnumType, TAttrType>
        where TEnumType : struct, IConvertible
        where TAttrType : Attribute
    {
        /// <summary>
        /// Create cache from enum
        /// </summary>
        /// <typeparam name="TEnumType">Enum type</typeparam>
        public static ReflectionCache<TEnumType, TAttrType> CreateFromEnum()
        {
            var enumType = typeof(TEnumType);

            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException("TEnumType must be an enumerated type");

            // Cache all types
            var r = new ReflectionCache<TEnumType, TAttrType>();

            foreach (var t in Enum.GetValues(enumType))
            {
                // Get enumn member
                var memInfo = enumType.GetMember(t.ToString());
                if (memInfo == null || memInfo.Length != 1)
                    throw (new FormatException());

                // Get attribute
                var attribute = memInfo[0].GetCustomAttributes<TAttrType>(false)
                    .FirstOrDefault();

                if (attribute == null)
                    continue;

                // Append to cache
                r.Add((TEnumType)t, attribute);
            }

            return r;
        }
    }
}
