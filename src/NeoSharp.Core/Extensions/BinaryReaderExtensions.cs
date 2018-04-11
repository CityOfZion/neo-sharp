using NeoSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NeoSharp.Core.Extensions
{
    public static class BinaryReaderExtensions
    {
        public static byte[] ReadBytesWithGrouping(this BinaryReader reader)
        {
            const int GROUP_SIZE = 16;
            using (MemoryStream ms = new MemoryStream())
            {
                int padding = 0;
                do
                {
                    byte[] group = reader.ReadBytes(GROUP_SIZE);
                    padding = reader.ReadByte();
                    if (padding > GROUP_SIZE)
                        throw new FormatException();
                    int count = GROUP_SIZE - padding;
                    if (count > 0)
                        ms.Write(group, 0, count);
                } while (padding == 0);
                return ms.ToArray();
            }
        }

        public static string ReadFixedString(this BinaryReader reader, int length)
        {
            byte[] data = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(data.TakeWhile(p => p != 0).ToArray());
        }

        public static T ReadSerializable<T>(this BinaryReader reader) where T : ISerializable, new()
        {
            T obj = new T();
            obj.Deserialize(reader);
            return obj;
        }

        public static T[] ReadSerializableArray<T>(this BinaryReader reader, int max = 0x10000000) where T : ISerializable, new()
        {
            T[] array = new T[reader.ReadVarInt((ulong)max)];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new T();
                array[i].Deserialize(reader);
            }
            return array;
        }

        public static byte[] ReadVarBytes(this BinaryReader reader, int max = 0X7fffffc7)
        {
            return reader.ReadBytes((int)reader.ReadVarInt((ulong)max));
        }

        public static ulong ReadVarInt(this BinaryReader reader, ulong max = ulong.MaxValue)
        {
            byte fb = reader.ReadByte();
            ulong value;
            if (fb == 0xFD)
                value = reader.ReadUInt16();
            else if (fb == 0xFE)
                value = reader.ReadUInt32();
            else if (fb == 0xFF)
                value = reader.ReadUInt64();
            else
                value = fb;
            if (value > max) throw new FormatException();
            return value;
        }

        public static string ReadVarString(this BinaryReader reader, int max = 0X7fffffc7)
        {
            return Encoding.UTF8.GetString(reader.ReadVarBytes(max));
        }
    }
}
