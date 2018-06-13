using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NeoSharp.BinarySerialization.Cache
{
    internal class BinarySerializerCacheEntry
    {
        // Delegates

        public delegate object ReadValueDelegate(IBinaryDeserializer deserializer, BinaryReader reader);
        public delegate int WriteValueDelegate(IBinarySerializer serializer, BinaryWriter writer, object value);

        public delegate object GetValueDelegate(object o);
        public delegate void SetValueDelegate(object o, object value);

        // Callbacks

        public readonly ReadValueDelegate ReadValue;
        public readonly WriteValueDelegate WriteValue;

        public readonly GetValueDelegate GetValue;
        public readonly SetValueDelegate SetValue;

        // Fields

        public readonly string Name;
        public readonly int MaxLength;
        public readonly bool ReadOnly;
        public readonly BinaryPropertyAttribute Context;

        // Cache

        private static Type _iListType = typeof(IList);
        private const byte BTRUE = 0x01;
        private const byte BFALSE = 0x00;

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
        /// <param name="btype">Type</param>
        /// <param name="member">Member</param>
        private BinarySerializerCacheEntry(BinaryPropertyAttribute atr, Type btype, MemberInfo member)
        {
            Name = member.Name;
            var type = btype;
            var isArray = type.IsArray;
            var isList = _iListType.IsAssignableFrom(type);

            if (atr == null)
            {
                Context = null;
                MaxLength = 0;
            }
            else
            {
                Context = atr;
                MaxLength = atr.MaxLength;
            }

            if (type == typeof(byte[]))
            {
                ReadValue = GetByteArrayValue;
                WriteValue = SetByteArrayValue;
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

                if (type == typeof(string))
                {
                    ReadValue = GetStringValue;
                    WriteValue = SetStringValue;
                }
                else if (type == typeof(long))
                {
                    ReadValue = GetInt64Value;
                    WriteValue = SetInt64Value;
                }
                else if (type == typeof(ulong))
                {
                    ReadValue = GetUInt64Value;
                    WriteValue = SetUInt64Value;
                }
                else if (type == typeof(int))
                {
                    ReadValue = GetInt32Value;
                    WriteValue = SetInt32Value;
                }
                else if (type == typeof(uint))
                {
                    ReadValue = GetUInt32Value;
                    WriteValue = SetUInt32Value;
                }
                else if (type == typeof(short))
                {
                    ReadValue = GetInt16Value;
                    WriteValue = SetInt16Value;
                }
                else if (type == typeof(ushort))
                {
                    ReadValue = GetUInt16Value;
                    WriteValue = SetUInt16Value;
                }
                else if (type == typeof(byte))
                {
                    ReadValue = GetByteValue;
                    WriteValue = SetByteValue;
                }
                else if (type == typeof(sbyte))
                {
                    ReadValue = GetSByteValue;
                    WriteValue = SetSByteValue;
                }
                else if (type == typeof(bool))
                {
                    ReadValue = GetBoolValue;
                    WriteValue = SetBoolValue;
                }
                else if (type == typeof(double))
                {
                    ReadValue = GetDoubleValue;
                    WriteValue = SetDoubleValue;
                }
                else if (!TryRecursive(type, out ReadValue, out WriteValue))
                {
                    throw new NotImplementedException();
                }

                if (isArray)
                {
                    var ar = new ArrayType(btype, MaxLength, ReadValue, WriteValue);
                    ReadValue = ar.GetArrayValue;
                    WriteValue = ar.SetArrayValue;
                }
                else if (isList)
                {
                    var ls = new ListType(btype, MaxLength, ReadValue, WriteValue);
                    ReadValue = ls.GetListValue;
                    WriteValue = ls.SetListValue;
                }
            }
        }

        #region Internals

        #region Recursive

        private class RecursiveType
        {
            private readonly Type Type;

            public RecursiveType(Type type)
            {
                Type = type;
            }

            public int SetRecursiveValue(IBinarySerializer serializer, BinaryWriter writer, object value)
            {
                return serializer.Serialize(value, writer);
            }

            public object GetRecursiveValue(IBinaryDeserializer deserializer, BinaryReader reader)
            {
                return deserializer.Deserialize(reader, Type);
            }
        }

        #endregion

        #region ByteArray

        private int SetByteArrayValue(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            var ar = (byte[])value;

            if (ar == null)
                return WriteVarInt(writer, 0);

            if (ar.Length > MaxLength)
                throw new FormatException("MaxLength");

            return WriteVarBytes(writer, ar);
        }

        private object GetByteArrayValue(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return ReadVarBytes(reader, MaxLength);
        }

        #endregion

        #region List

        private class ListType
        {
            private readonly Type Type;
            private readonly ReadValueDelegate GetValue;
            private readonly WriteValueDelegate SetValue;
            private readonly int MaxLength;

            public ListType(Type type, int maxLength, ReadValueDelegate get, WriteValueDelegate set)
            {
                MaxLength = maxLength;
                GetValue = get;
                SetValue = set;
                Type = type;
            }

            public int SetListValue(IBinarySerializer serializer, BinaryWriter writer, object value)
            {
                var ar = (IList)value;

                if (ar == null)
                {
                    return WriteVarInt(writer, 0);
                }

                var x = WriteVarInt(writer, ar.Count);

                if (x > MaxLength) throw new FormatException("MaxLength");

                foreach (var o in ar)
                    x += SetValue(serializer, writer, o);

                return x;
            }

            public object GetListValue(IBinaryDeserializer deserializer, BinaryReader reader)
            {
                var l = (int)ReadVarInt(reader, ushort.MaxValue);
                if (l > MaxLength) throw new FormatException("MaxLength");

                var a = (IList)Activator.CreateInstance(Type);

                for (var ix = 0; ix < l; ix++)
                {
                    a.Add(GetValue(deserializer, reader));
                }

                return a;
            }
        }

        #endregion

        #region Array

        private class ArrayType
        {
            private readonly Type Type;
            private readonly ReadValueDelegate GetValue;
            private readonly WriteValueDelegate SetValue;
            private readonly int MaxLength;

            public ArrayType(Type type, int maxLength, ReadValueDelegate get, WriteValueDelegate set)
            {
                MaxLength = maxLength;
                GetValue = get;
                SetValue = set;
                Type = type;
            }

            public int SetArrayValue(IBinarySerializer serializer, BinaryWriter writer, object value)
            {
                var ar = (Array)value;

                if (ar == null)
                {
                    return WriteVarInt(writer, 0);
                }

                var x = WriteVarInt(writer, ar.Length);

                if (x > MaxLength) throw new FormatException("MaxLength");

                foreach (var o in ar)
                    x += SetValue(serializer, writer, o);

                return x;
            }

            public object GetArrayValue(IBinaryDeserializer deserializer, BinaryReader reader)
            {
                var l = (int)ReadVarInt(reader, ushort.MaxValue);
                if (l > MaxLength) throw new FormatException("MaxLength");

                var a = (Array)Activator.CreateInstance(Type, l);

                for (var ix = 0; ix < l; ix++)
                {
                    a.SetValue(GetValue(deserializer, reader), ix);
                }

                return a;
            }
        }

        #endregion

        #region String

        private int SetStringValue(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            var data = Encoding.UTF8.GetBytes((string)value);

            if (data.Length >= MaxLength)
                throw new FormatException("MaxLength");

            return WriteVarBytes(writer, data);
        }

        private object GetStringValue(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return ReadVarString(reader, MaxLength);
        }

        #endregion

        #region Int64

        private int SetInt64Value(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            writer.Write((long)value);
            return 8;
        }

        private object GetInt64Value(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return reader.ReadInt64();
        }

        #endregion

        #region UInt64

        private int SetUInt64Value(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            writer.Write((ulong)value);
            return 8;
        }

        private object GetUInt64Value(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return reader.ReadUInt64();
        }

        #endregion

        #region Int32

        private int SetInt32Value(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            writer.Write((int)value);
            return 4;
        }

        private object GetInt32Value(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        #endregion

        #region UInt32

        private int SetUInt32Value(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            writer.Write((uint)value);
            return 4;
        }

        private object GetUInt32Value(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        #endregion

        #region Int16

        private int SetInt16Value(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            writer.Write((short)value);
            return 2;
        }

        private object GetInt16Value(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return reader.ReadInt16();
        }

        #endregion

        #region UInt16

        private int SetUInt16Value(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            writer.Write((ushort)value);
            return 2;
        }

        private object GetUInt16Value(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        #endregion

        #region Byte

        private int SetByteValue(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            writer.Write((byte)value);
            return 1;
        }

        private object GetByteValue(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return reader.ReadByte();
        }

        #endregion

        #region SByte

        private int SetSByteValue(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            writer.Write((sbyte)value);
            return 1;
        }

        private object GetSByteValue(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        #endregion

        #region Bool

        private int SetBoolValue(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            if ((bool)value) writer.Write(BTRUE);
            else writer.Write(BFALSE);

            return 1;
        }

        private object GetBoolValue(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return reader.ReadByte() != 0x00;
        }

        #endregion

        #region Double

        private int SetDoubleValue(IBinarySerializer serializer, BinaryWriter writer, object value)
        {
            writer.Write((double)value);
            return 8;
        }

        private object GetDoubleValue(IBinaryDeserializer deserializer, BinaryReader reader)
        {
            return reader.ReadDouble();
        }

        #endregion

        #endregion

        #region Helpers

        private static bool TryRecursive(Type type, out ReadValueDelegate readValue, out WriteValueDelegate writeValue)
        {
            var cache = BinarySerializerCache.InternalRegisterTypes(type);
            if (cache == null)
            {
                foreach (var typeConverter in BinarySerializerCache.TypeConverterCache.Values)
                {
                    if (typeConverter.CanConvertTo(typeof(byte[])) && typeConverter.CanConvertFrom(type))
                    {
                        if (typeConverter is IFixedBufferConverter fix)
                        {
                            int bufferLength = fix.FixedLength;

                            readValue = (deserializer, reader) =>
                            {
                                var buffer = reader.ReadBytes(bufferLength);
                                return typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, buffer);
                            };

                            writeValue = (serializer, writer, value) =>
                            {
                                var buffer = (byte[])typeConverter.ConvertTo(value, typeof(byte[]));
                                writer.Write(buffer);
                                return buffer.Length;
                            };
                        }
                        else
                        {
                            readValue = (deserializer, reader) =>
                            {
                                var buffer = ReadVarBytes(reader, 100);
                                return typeConverter.ConvertFrom(null, CultureInfo.InvariantCulture, buffer);
                            };

                            writeValue = (serializer, writer, value) =>
                            {
                                var buffer = (byte[])typeConverter.ConvertTo(value, typeof(byte[]));
                                return WriteVarBytes(writer, buffer);
                            };
                        }

                        return true;
                    }
                }

                readValue = null;
                writeValue = null;
                return false;
            }

            var r = new RecursiveType(type);
            readValue = r.GetRecursiveValue;
            writeValue = r.SetRecursiveValue;
            return true;
        }

        private static byte[] ReadVarBytes(BinaryReader reader, int max = 0X7fffffc7)
        {
            return reader.ReadBytes((int)ReadVarInt(reader, (ulong)max));
        }

        private static ulong ReadVarInt(BinaryReader reader, ulong max = ulong.MaxValue)
        {
            var fb = reader.ReadByte();
            ulong value;
            if (fb == 0xFD)
                value = reader.ReadUInt16();
            else if (fb == 0xFE)
                value = reader.ReadUInt32();
            else if (fb == 0xFF)
                value = reader.ReadUInt64();
            else
                value = fb;

            if (value > max)
                throw new FormatException("MaxLength");

            return value;
        }

        private static string ReadVarString(BinaryReader reader, int max = 0X7fffffc7)
        {
            return Encoding.UTF8.GetString(ReadVarBytes(reader, max));
        }

        public static int WriteVarBytes(BinaryWriter writer, byte[] value)
        {
            if (value == null)
            {
                return WriteVarInt(writer, 0);
            }
            else
            {
                var ret = WriteVarInt(writer, value.Length);
                writer.Write(value);
                return ret + value.Length;
            }
        }

        public static int WriteVarInt(BinaryWriter writer, long value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException();

            if (value < 0xFD)
            {
                writer.Write((byte)value);
                return 1;
            }
            else if (value <= 0xFFFF)
            {
                writer.Write((byte)0xFD);
                writer.Write((ushort)value);
                return 3;
            }
            else if (value <= 0xFFFFFFFF)
            {
                writer.Write((byte)0xFE);
                writer.Write((uint)value);
                return 5;
            }
            else
            {
                writer.Write((byte)0xFF);
                writer.Write(value);
                return 9;
            }
        }

        public static void WriteVarString(BinaryWriter writer, string value)
        {
            WriteVarBytes(writer, Encoding.UTF8.GetBytes(value));
        }

        #endregion

        public override string ToString()
        {
            return Name;
        }
    }
}