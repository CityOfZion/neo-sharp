using System;
using System.Collections;
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
        public readonly bool ReadOnly;
        public readonly BinaryPropertyAttribute Context;

        // Cache

        private static Type _iListType = typeof(IList);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="atr">Attribute</param>
        /// <param name="pi">Property</param>
        public BinarySerializerCacheEntry(BinaryPropertyAttribute atr, PropertyInfo pi) : this(atr, pi.PropertyType, pi)
        {
            GetValue = pi.GetValue;
            SetValue = pi.SetValue;
            ReadOnly = !pi.CanWrite;
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
            ReadOnly = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="atr">Attribute</param>
        /// <param name="type">Type</param>
        /// <param name="member">Member</param>
        public BinarySerializerCacheEntry(BinaryPropertyAttribute atr, Type type, MemberInfo member)
        {
            Name = member.Name ?? null;

            Type = type;
            var isArray = type.IsArray;
            var isList = _iListType.IsAssignableFrom(type);

            if (atr == null)
            {
                Context = null;
                MaxLength = 0;
                Order = 0;
            }
            else
            {
                Context = atr;
                MaxLength = atr.MaxLength;
                Order = atr.Order;
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

                if (type.IsEnum)
                {
                    type = Enum.GetUnderlyingType(type);
                }

                // Try to extract the BinarySerializer

                var serializerAttr = member.GetCustomAttribute<BinaryTypeSerializerAttribute>();
                if (serializerAttr == null)
                    serializerAttr = type.GetCustomAttribute<BinaryTypeSerializerAttribute>();

                if (serializerAttr != null) Serializer = serializerAttr.Create();
                else if (type == typeof(string)) Serializer = new BinaryStringSerializer(MaxLength);
                else if (type == typeof(long)) Serializer = new BinaryInt64Serializer();
                else if (type == typeof(ulong)) Serializer = new BinaryUInt64Serializer();
                else if (type == typeof(int)) Serializer = new BinaryInt32Serializer();
                else if (type == typeof(uint)) Serializer = new BinaryUInt32Serializer();
                else if (type == typeof(short)) Serializer = new BinaryInt16Serializer();
                else if (type == typeof(ushort)) Serializer = new BinaryUInt16Serializer();
                else if (type == typeof(byte)) Serializer = new BinaryByteSerializer();
                else if (type == typeof(sbyte)) Serializer = new BinarySByteSerializer();
                else if (type == typeof(bool)) Serializer = new BinaryBoolSerializer();
                else if (type == typeof(double)) Serializer = new BinaryDoubleSerializer();
                else if (!TryRecursive(type, out Serializer)) throw new NotImplementedException();

                if (isArray)
                {
                    Serializer = new BinaryArraySerializer(Type, Serializer, MaxLength);
                }
                else if (isList)
                {
                    Serializer = new BinaryListSerializer(Type, Serializer, MaxLength);
                }

                if (Serializer == null)
                {
                    throw new NotImplementedException();
                }
            }
        }

        #region Helpers

        private static bool TryRecursive(Type type, out IBinaryCustomSerializable serializer)
        {
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