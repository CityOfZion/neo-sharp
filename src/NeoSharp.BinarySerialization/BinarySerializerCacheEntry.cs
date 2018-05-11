using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;

namespace NeoSharp.BinarySerialization
{
    internal class BinarySerializerCacheEntry
    {
        // Delegates

        public delegate object delReadValue(BinaryReader reader);
        public delegate int delWriteValue(BinaryWriter writer, object value);

        public delegate object delGetValue(object o);
        public delegate void delSetValue(object o, object value);

        // Callbacks

        public readonly delReadValue ReadValue;
        public readonly delWriteValue WriteValue;

        public readonly delGetValue GetValue;
        public readonly delSetValue SetValue;

        // Fields

        public readonly string Name;
        public readonly int MaxLength;
        public readonly bool ReadOnly;

        // Cache

        static Type _iListType = typeof(IList);
        const byte BTRUE = 0x01;
        const byte BFALSE = 0x00;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="atr">Attribute</param>
        /// <param name="pi">Property</param>
        public BinarySerializerCacheEntry(BinaryPropertyAttribute atr, PropertyInfo pi) : this(atr, pi.PropertyType)
        {
            Name = pi.Name;
            GetValue = new delGetValue(pi.GetValue);
            SetValue = new delSetValue(pi.SetValue);
            ReadOnly = !pi.CanWrite;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="atr">Attribute</param>
        /// <param name="fi">Field</param>
        public BinarySerializerCacheEntry(BinaryPropertyAttribute atr, FieldInfo fi) : this(atr, fi.FieldType)
        {
            Name = fi.Name;
            GetValue = new delGetValue(fi.GetValue);
            SetValue = new delSetValue(fi.SetValue);
            ReadOnly = false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="atr">Attribute</param>
        /// <param name="btype">Type</param>
        BinarySerializerCacheEntry(BinaryPropertyAttribute atr, Type btype)
        {
            Type type = btype;
            MaxLength = atr.MaxLength;
            bool array = type.IsArray;
            bool list = _iListType.IsAssignableFrom(type);

            if (type == typeof(byte[]))
            {
                ReadValue = new delReadValue(GetByteArrayValue);
                WriteValue = new delWriteValue(SetByteArrayValue);
            }
            else
            {
                if (array || list)
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
                    ReadValue = new delReadValue(GetStringValue);
                    WriteValue = new delWriteValue(SetStringValue);
                }
                else if (type == typeof(long))
                {
                    ReadValue = new delReadValue(GetInt64Value);
                    WriteValue = new delWriteValue(SetInt64Value);
                }
                else if (type == typeof(ulong))
                {
                    ReadValue = new delReadValue(GetUInt64Value);
                    WriteValue = new delWriteValue(SetUInt64Value);
                }
                else if (type == typeof(int))
                {
                    ReadValue = new delReadValue(GetInt32Value);
                    WriteValue = new delWriteValue(SetInt32Value);
                }
                else if (type == typeof(uint))
                {
                    ReadValue = new delReadValue(GetUInt32Value);
                    WriteValue = new delWriteValue(SetUInt32Value);
                }
                else if (type == typeof(short))
                {
                    ReadValue = new delReadValue(GetInt16Value);
                    WriteValue = new delWriteValue(SetInt16Value);
                }
                else if (type == typeof(ushort))
                {
                    ReadValue = new delReadValue(GetUInt16Value);
                    WriteValue = new delWriteValue(SetUInt16Value);
                }
                else if (type == typeof(byte))
                {
                    ReadValue = new delReadValue(GetByteValue);
                    WriteValue = new delWriteValue(SetByteValue);
                }
                else if (type == typeof(sbyte))
                {
                    ReadValue = new delReadValue(GetSByteValue);
                    WriteValue = new delWriteValue(SetSByteValue);
                }
                else if (type == typeof(bool))
                {
                    ReadValue = new delReadValue(GetBoolValue);
                    WriteValue = new delWriteValue(SetBoolValue);
                }
                else if (type == typeof(double))
                {
                    ReadValue = new delReadValue(GetDoubleValue);
                    WriteValue = new delWriteValue(SetDoubleValue);
                }
                else if (!TryRecursive(type, out ReadValue, out WriteValue))
                {
                    throw (new NotImplementedException());
                }

                if (array)
                {
                    ArrayType ar = new ArrayType(btype, MaxLength, ReadValue, WriteValue);
                    ReadValue = new delReadValue(ar.GetArrayValue);
                    WriteValue = new delWriteValue(ar.SetArrayValue);
                }
                else if (list)
                {
                    ListType ls = new ListType(btype, MaxLength, ReadValue, WriteValue);
                    ReadValue = new delReadValue(ls.GetListValue);
                    WriteValue = new delWriteValue(ls.SetListValue);
                }
            }
        }

        #region Internals

        #region Recursive

        class RecursiveType
        {
            readonly Type Type;

            public RecursiveType(Type type)
            {
                Type = type;
            }

            public int SetRecursiveValue(BinaryWriter writer, object value)
            {
                return BinarySerializer.Serialize(value, writer);
            }

            public object GetRecursiveValue(BinaryReader reader)
            {
                return BinarySerializer.Deserialize(reader, Type);
            }
        }

        #endregion

        #region ByteArray

        public int SetByteArrayValue(BinaryWriter writer, object value)
        {
            byte[] ar = (byte[])value;

            if (ar == null)
                return WriteVarInt(writer, 0);

            if (ar.Length > MaxLength) throw new FormatException("MaxLength");
            return WriteVarBytes(writer, ar);
        }

        public object GetByteArrayValue(BinaryReader reader)
        {
            return ReadVarBytes(reader, MaxLength);
        }

        #endregion

        #region List

        class ListType
        {
            readonly Type Type;
            readonly delReadValue GetValue;
            readonly delWriteValue SetValue;
            readonly int MaxLength;

            public ListType(Type type, int maxLength, delReadValue get, delWriteValue set)
            {
                MaxLength = maxLength;
                GetValue = get;
                SetValue = set;
                Type = type;
            }

            public int SetListValue(BinaryWriter writer, object value)
            {
                var ar = (IList)value;

                if (ar == null)
                {
                    return WriteVarInt(writer, 0);
                }

                int x = WriteVarInt(writer, ar.Count);

                if (x > MaxLength) throw new FormatException("MaxLength");

                foreach (object o in ar)
                    x += SetValue(writer, o);

                return x;
            }

            public object GetListValue(BinaryReader reader)
            {
                int l = (int)ReadVarInt(reader, ushort.MaxValue);
                if (l > MaxLength) throw new FormatException("MaxLength");

                var a = (IList)Activator.CreateInstance(Type);

                for (int ix = 0; ix < l; ix++)
                {
                    a.Add(GetValue(reader));
                }

                return a;
            }
        }

        #endregion

        #region Array

        class ArrayType
        {
            readonly Type Type;
            readonly delReadValue GetValue;
            readonly delWriteValue SetValue;
            readonly int MaxLength;

            public ArrayType(Type type, int maxLength, delReadValue get, delWriteValue set)
            {
                MaxLength = maxLength;
                GetValue = get;
                SetValue = set;
                Type = type;
            }

            public int SetArrayValue(BinaryWriter writer, object value)
            {
                var ar = (Array)value;

                if (ar == null)
                {
                    return WriteVarInt(writer, 0);
                }

                int x = WriteVarInt(writer, ar.Length);

                if (x > MaxLength) throw new FormatException("MaxLength");

                foreach (object o in ar)
                    x += SetValue(writer, o);

                return x;
            }

            public object GetArrayValue(BinaryReader reader)
            {
                int l = (int)ReadVarInt(reader, ushort.MaxValue);
                if (l > MaxLength) throw new FormatException("MaxLength");

                var a = (Array)Activator.CreateInstance(Type, l);

                for (int ix = 0; ix < l; ix++)
                {
                    a.SetValue(GetValue(reader), ix);
                }

                return a;
            }
        }

        #endregion

        #region String

        private int SetStringValue(BinaryWriter writer, object value)
        {
            byte[] data = Encoding.UTF8.GetBytes((string)value);

            if (data.Length >= MaxLength)
                throw new FormatException("MaxLength");

            return WriteVarBytes(writer, data);
        }

        private object GetStringValue(BinaryReader reader)
        {
            return ReadVarString(reader, MaxLength);
        }

        #endregion

        #region Int64

        private int SetInt64Value(BinaryWriter writer, object value)
        {
            writer.Write((long)value);
            return 8;
        }

        private object GetInt64Value(BinaryReader reader)
        {
            return reader.ReadInt64();
        }

        #endregion

        #region UInt64

        private int SetUInt64Value(BinaryWriter writer, object value)
        {
            writer.Write((ulong)value);
            return 8;
        }

        private object GetUInt64Value(BinaryReader reader)
        {
            return reader.ReadUInt64();
        }

        #endregion

        #region Int32

        private int SetInt32Value(BinaryWriter writer, object value)
        {
            writer.Write((int)value);
            return 4;
        }

        private object GetInt32Value(BinaryReader reader)
        {
            return reader.ReadInt32();
        }

        #endregion

        #region UInt32

        private int SetUInt32Value(BinaryWriter writer, object value)
        {
            writer.Write((uint)value);
            return 4;
        }

        private object GetUInt32Value(BinaryReader reader)
        {
            return reader.ReadUInt32();
        }

        #endregion

        #region Int16

        private int SetInt16Value(BinaryWriter writer, object value)
        {
            writer.Write((short)value);
            return 2;
        }

        private object GetInt16Value(BinaryReader reader)
        {
            return reader.ReadInt16();
        }

        #endregion

        #region UInt16

        private int SetUInt16Value(BinaryWriter writer, object value)
        {
            writer.Write((ushort)value);
            return 2;
        }

        private object GetUInt16Value(BinaryReader reader)
        {
            return reader.ReadUInt16();
        }

        #endregion

        #region Byte

        private int SetByteValue(BinaryWriter writer, object value)
        {
            writer.Write((byte)value);
            return 1;
        }

        private object GetByteValue(BinaryReader reader)
        {
            return reader.ReadByte();
        }

        #endregion

        #region SByte

        private int SetSByteValue(BinaryWriter writer, object value)
        {
            writer.Write((sbyte)value);
            return 1;
        }

        private object GetSByteValue(BinaryReader reader)
        {
            return reader.ReadSByte();
        }

        #endregion

        #region Bool

        private int SetBoolValue(BinaryWriter writer, object value)
        {
            if ((bool)value) writer.Write(BTRUE);
            else writer.Write(BFALSE);

            return 1;
        }

        private object GetBoolValue(BinaryReader reader)
        {
            return reader.ReadByte() != 0x00;
        }

        #endregion

        #region Double

        private int SetDoubleValue(BinaryWriter writer, object value)
        {
            writer.Write((double)value);
            return 8;
        }

        private object GetDoubleValue(BinaryReader reader)
        {
            return reader.ReadDouble();
        }

        #endregion

        #endregion

        #region Helpers

        static bool TryRecursive(Type btype, out delReadValue readValue, out delWriteValue writeValue)
        {
            BinarySerializerCache cache = BinarySerializer.InternalCacheTypesOf(btype);
            if (cache == null)
            {
                readValue = null;
                writeValue = null;
                return false;
            }

            RecursiveType r = new RecursiveType(btype);
            readValue = r.GetRecursiveValue;
            writeValue = r.SetRecursiveValue;
            return true;
        }

        static byte[] ReadVarBytes(BinaryReader reader, int max = 0X7fffffc7)
        {
            return reader.ReadBytes((int)ReadVarInt(reader, (ulong)max));
        }

        static ulong ReadVarInt(BinaryReader reader, ulong max = ulong.MaxValue)
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

        static string ReadVarString(BinaryReader reader, int max = 0X7fffffc7)
        {
            return Encoding.UTF8.GetString(ReadVarBytes(reader, max));
        }

        public static int WriteVarBytes(BinaryWriter writer, byte[] value)
        {
            int ret = WriteVarInt(writer, value.Length);
            writer.Write(value);
            return ret + value.Length;
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
            return Name.ToString();
        }
    }
}