using NeoSharp.Core.Types;
using System;
using System.IO;
using System.Text;

namespace NeoSharp.Core.Extensions
{
    public static class BinaryWriterExtensions
    {
        public static void Write(this BinaryWriter writer, ISerializable value)
        {
            value.Serialize(writer);
        }

        public static void Write<T>(this BinaryWriter writer, T[] value) where T : ISerializable
        {
            writer.WriteVarInt(value.Length);
            for (var i = 0; i < value.Length; i++)
            {
                value[i].Serialize(writer);
            }
        }

        public static void WriteBytesWithGrouping(this BinaryWriter writer, byte[] value)
        {
            const int groupSize = 16;
            var index = 0;
            var remain = value.Length;
            while (remain >= groupSize)
            {
                writer.Write(value, index, groupSize);
                writer.Write((byte)0);
                index += groupSize;
                remain -= groupSize;
            }
            if (remain > 0)
                writer.Write(value, index, remain);
            var padding = groupSize - remain;
            for (var i = 0; i < padding; i++)
                writer.Write((byte)0);
            writer.Write((byte)padding);
        }

        public static void WriteFixedString(this BinaryWriter writer, string value, int length)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value.Length > length)
                throw new ArgumentException();
            var bytes = Encoding.UTF8.GetBytes(value);
            if (bytes.Length > length)
                throw new ArgumentException();
            writer.Write(bytes);
            if (bytes.Length < length)
                writer.Write(new byte[length - bytes.Length]);
        }

        public static int WriteVarBytes(this BinaryWriter writer, byte[] value)
        {
            int ret = writer.WriteVarInt(value.Length);
            writer.Write(value);
            return ret + value.Length;
        }

        public static int WriteVarInt(this BinaryWriter writer, long value)
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

        public static void WriteVarString(this BinaryWriter writer, string value)
        {
            writer.WriteVarBytes(Encoding.UTF8.GetBytes(value));
        }
    }
}
