using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToHexString(this IEnumerable<byte> value)
        {
            var sb = new StringBuilder();
            foreach (var b in value)
                sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        public static T AsSerializable<T>(this byte[] value, int start = 0) where T : ISerializable, new()
        {
            using (var ms = new MemoryStream(value, start, value.Length - start, false))
            using (var reader = new BinaryReader(ms, Encoding.UTF8))
            {
                return reader.ReadSerializable<T>();
            }
        }

        public static ISerializable AsSerializable(this byte[] value, Type type)
        {
            if (!typeof(ISerializable).GetTypeInfo().IsAssignableFrom(type))
                throw new InvalidCastException();
            var serializable = (ISerializable)Activator.CreateInstance(type);
            using (var ms = new MemoryStream(value, false))
            using (var reader = new BinaryReader(ms, Encoding.UTF8))
            {
                serializable.Deserialize(reader);
            }
            return serializable;
        }

        public static T[] AsSerializableArray<T>(this byte[] value, int max = 0x10000000) where T : ISerializable, new()
        {
            using (var ms = new MemoryStream(value, false))
            using (var reader = new BinaryReader(ms, Encoding.UTF8))
            {
                return reader.ReadSerializableArray<T>(max);
            }
        }
    }
}
