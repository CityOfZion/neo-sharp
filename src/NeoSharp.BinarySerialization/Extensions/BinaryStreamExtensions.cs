using System;
using System.IO;
using System.Text;

namespace NeoSharp.BinarySerialization.Extensions
{
    public static class BinaryStreamExtensions
    {
        public static byte[] ReadVarBytes(this BinaryReader reader, int max = 0X7fffffc7)
        {
            return reader.ReadBytes((int)ReadVarInt(reader, (ulong)max));
        }

        public static ulong ReadVarInt(this BinaryReader reader, ulong max = ulong.MaxValue)
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

        public static string ReadVarString(this BinaryReader reader, int max = 0X7fffffc7)
        {
            return Encoding.UTF8.GetString(ReadVarBytes(reader, max));
        }

        public static int WriteVarBytes(this BinaryWriter writer, byte[] value)
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
            WriteVarBytes(writer, Encoding.UTF8.GetBytes(value));
        }
    }
}