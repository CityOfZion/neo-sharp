using System;
using System.Linq;

namespace NeoSharp.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsAssignableToGenericType(this Type source, Type openGenericType)
        {
            var interfaceTypes = source.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == openGenericType))
            {
                return true;
            }

            if (source.IsGenericType && source.GetGenericTypeDefinition() == openGenericType)
                return true;

            var baseType = source.BaseType;

            return baseType != null && IsAssignableToGenericType(baseType, openGenericType);
        }
    }
}