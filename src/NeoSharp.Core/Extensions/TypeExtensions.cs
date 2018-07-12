using System;
using System.Linq;

namespace NeoSharp.Core.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Checks if a type is assignable to a type
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="openGenericType">Type</param>
        /// <returns>True if assignable</returns>
        public static bool IsAssignableToGenericType(this Type source, Type openGenericType)
        {
            var interfaceTypes = source.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == openGenericType))
            {
                return true;
            }

            if (source.IsGenericType && source.GetGenericTypeDefinition() == openGenericType)
            {
                return true;
            }

            var baseType = source.BaseType;

            return baseType != null && IsAssignableToGenericType(baseType, openGenericType);
        }
    }
}