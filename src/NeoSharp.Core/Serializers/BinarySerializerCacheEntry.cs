using System.IO;
using System.Reflection;
using System.Text;

namespace NeoSharp.Core.Serializers
{
    internal class BinarySerializerCacheEntry
    {
        // Delegates

        public delegate object delGetValue(BinaryReader reader);
        public delegate int delSetValue(BinaryWriter writer, object value);

        /// <summary>
        /// Property
        /// </summary>
        public readonly PropertyInfo Property;

        // Callbacks

        public readonly delGetValue GetValue;
        public readonly delSetValue SetValue;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pi">Property</param>
        public BinarySerializerCacheEntry(PropertyInfo pi)
        {
            Property = pi;

            if (pi.PropertyType == typeof(string))
            {
                GetValue = new delGetValue(GetStringValue);
                SetValue = new delSetValue(SetStringValue);
            }
            else if (pi.PropertyType == typeof(long))
            {
                GetValue = new delGetValue(GetInt64Value);
                SetValue = new delSetValue(SetInt64Value);
            }
            else if (pi.PropertyType == typeof(ulong))
            {
                GetValue = new delGetValue(GetUInt64Value);
                SetValue = new delSetValue(SetUInt64Value);
            }
            else if (pi.PropertyType == typeof(int))
            {
                GetValue = new delGetValue(GetInt32Value);
                SetValue = new delSetValue(SetInt32Value);
            }
            else if (pi.PropertyType == typeof(uint))
            {
                GetValue = new delGetValue(GetUInt32Value);
                SetValue = new delSetValue(SetUInt32Value);
            }
            else if (pi.PropertyType == typeof(short))
            {
                GetValue = new delGetValue(GetInt16Value);
                SetValue = new delSetValue(SetInt16Value);
            }
            else if (pi.PropertyType == typeof(ushort))
            {
                GetValue = new delGetValue(GetUInt16Value);
                SetValue = new delSetValue(SetUInt16Value);
            }
            else if (pi.PropertyType == typeof(byte))
            {
                GetValue = new delGetValue(GetByteValue);
                SetValue = new delSetValue(SetByteValue);
            }
            else if (pi.PropertyType == typeof(sbyte))
            {
                GetValue = new delGetValue(GetSByteValue);
                SetValue = new delSetValue(SetSByteValue);
            }
        }

        #region Internals

        #region String

        private int SetStringValue(BinaryWriter writer, object value)
        {
            // TODO: check this logic

            byte[] data = Encoding.UTF8.GetBytes(value + "\0");
            writer.Write(data);
            return data.Length;
        }

        private object GetStringValue(BinaryReader reader)
        {
            return reader.ReadString();
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

        #endregion
    }
}