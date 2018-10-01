using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NeoSharp.BinarySerialization.SerializationHooks;
using NeoSharp.BinarySerialization.Serializers;

namespace NeoSharp.BinarySerialization.Cache
{
    internal class BinarySerializerCacheEntry
    {
        // Delegates

        public delegate object GetValueDelegate(object o);
        public delegate void SetValueDelegate(object o, object value);

        // Callbacks

        public readonly IBinaryCustomSerializable Serializer;

        public readonly GetValueDelegate GetValue;
        public readonly SetValueDelegate SetValue;

        // Fields

        public readonly int Order;
        public readonly Type Type;
        public readonly string Name;
        public readonly int MaxLength;
        public readonly ValueHandlerLogicType ValueHandlerLogic;
        public readonly bool Override;
        public readonly BinaryPropertyAttribute Context;

        // Cache

        private static Type _iListType = typeof(IList);
        private static Type[] _iReadonlyListType = new Type[] { typeof(IReadOnlyList<>), typeof(IReadOnlyCollection<>) };
        private static Type[] _iHashSetType = new Type[] { typeof(HashSet<>)/*, typeof(ISet<>)*/ };
        private static Type[] _iDictionaryTypes = new Type[] { typeof(Dictionary<,>)/*, typeof(IDictionary<,>)*/ };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="atr">Attribute</param>
        /// <param name="pi">Property</param>
        public BinarySerializerCacheEntry(BinaryPropertyAttribute atr, PropertyInfo pi) : this(atr, pi.PropertyType, pi)
        {
            GetValue = pi.GetValue;
            SetValue = pi.SetValue;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="atr">Attribute</param>
        /// <param name="fi">Field</param>
        public BinarySerializerCacheEntry(BinaryPropertyAttribute atr, FieldInfo fi) : this(atr, fi.FieldType, fi)
        {
            GetValue = fi.GetValue;
            SetValue = fi.SetValue;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="atr">Attribute</param>
        /// <param name="type">Type</param>
        /// <param name="member">Member</param>
        public BinarySerializerCacheEntry(BinaryPropertyAttribute atr, Type type, MemberInfo member)
        {
            Type = type;
            Name = member.Name ?? null;

            var isArray = type.IsArray;
            var isList = _iListType.IsAssignableFrom(type);
            var isReadOnlyList = type.IsGenericType && _iReadonlyListType.Contains(type.GetGenericTypeDefinition());
            var isHashSet = type.IsGenericType && _iHashSetType.Contains(type.GetGenericTypeDefinition());
            var isDic = type.IsGenericType && _iDictionaryTypes.Contains(type.GetGenericTypeDefinition());

            if (atr == null)
            {
                Context = null;
                MaxLength = Order = 0;
                Override = false;
                ValueHandlerLogic = ValueHandlerLogicType.Writable;
            }
            else
            {
                Override = atr.Override;
                Context = atr;
                MaxLength = atr.MaxLength;
                Order = atr.Order;
                ValueHandlerLogic = atr.ValueHandlerLogic;
            }

            if (type == typeof(byte[]))
            {
                Serializer = new BinaryByteArraySerializer(MaxLength);
            }
            else
            {
                if (isArray || isList)
                {
                    // Extract type of array

                    type = type.GetElementType();
                }
                else
                {
                    if (isHashSet || isReadOnlyList)
                    {
                        // Extract type of generic type

                        type = type.GetGenericArguments().FirstOrDefault();
                    }
                    else
                    {
                        if (isDic)
                        {
                            // Is dictionary

                            var gen = type.GetGenericArguments();

                            if (!TryRecursive(member, gen[0], out var key, MaxLength) ||
                                !TryRecursive(member, gen[1], out var value, MaxLength))
                            {
                                throw new NotImplementedException();
                            }

                            Serializer = new BinaryDictionarySerializer(type, gen[0], key, gen[1], value, MaxLength);
                            return;
                        }
                    }
                }

                var isEnum = type.IsEnum;

                if (isEnum)
                {
                    type = Enum.GetUnderlyingType(type);
                }

                // Try to extract the BinarySerializer

                if (!TryRecursive(member, type, out Serializer, MaxLength))
                {
                    throw new NotImplementedException();
                }

                if (isArray)
                {
                    Serializer = new BinaryArraySerializer(Type, Serializer, MaxLength);
                }
                else if (isList)
                {
                    Serializer = new BinaryListSerializer(Type, Serializer, MaxLength);
                }
                else if (isReadOnlyList)
                {
                    Serializer = new BinaryReadOnlyListSerializer(Type, Serializer, MaxLength);
                }
                else if (isHashSet)
                {
                    Serializer = new BinaryHashSetSerializer(Type, Serializer, MaxLength);
                }
                else if (isEnum)
                {
                    Serializer = new BinaryEnumSerializer(Type, Serializer);
                }

                if (Serializer == null)
                {
                    throw new NotImplementedException();
                }
            }
        }

        #region Helpers

        internal static bool TryRecursive(MemberInfo member, Type type, out IBinaryCustomSerializable serializer, int maxLength)
        {
            // Default types

            var serializerAttr = member?.GetCustomAttribute<BinaryTypeSerializerAttribute>();

            if (serializerAttr == null)
            {
                serializerAttr = type.GetCustomAttribute<BinaryTypeSerializerAttribute>();
            }

            if (serializerAttr != null)
            {
                serializer = serializerAttr.Create(); return true;
            }
            else if (type == typeof(string))
            {
                serializer = new BinaryStringSerializer(maxLength);
                return true;
            }
            else if (type == typeof(long))
            {
                serializer = new BinaryInt64Serializer();
                return true;
            }
            else if (type == typeof(ulong))
            {
                serializer = new BinaryUInt64Serializer();
                return true;
            }
            else if (type == typeof(int))
            {
                serializer = new BinaryInt32Serializer();
                return true;
            }
            else if (type == typeof(uint))
            {
                serializer = new BinaryUInt32Serializer();
                return true;
            }
            else if (type == typeof(short))
            {
                serializer = new BinaryInt16Serializer();
                return true;
            }
            else if (type == typeof(ushort))
            {
                serializer = new BinaryUInt16Serializer();
                return true;
            }
            else if (type == typeof(byte))
            {
                serializer = new BinaryByteSerializer();
                return true;
            }
            else if (type == typeof(sbyte))
            {
                serializer = new BinarySByteSerializer();
                return true;
            }
            else if (type == typeof(bool))
            {
                serializer = new BinaryBoolSerializer();
                return true;
            }
            else if (type == typeof(double))
            {
                serializer = new BinaryDoubleSerializer();
                return true;
            }

            // Search in cache

            var cache = BinarySerializerCache.InternalRegisterTypes(type);

            if (cache == null)
            {
                serializer = null;
                return false;
            }

            serializer = new RecursiveCustomSerializer(type);
            return true;
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}