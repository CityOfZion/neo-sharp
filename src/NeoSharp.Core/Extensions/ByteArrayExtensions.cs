using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using NeoSharp.Core.Types;
using System.Threading;
using System.Security.Cryptography;
using System.Linq;

namespace NeoSharp.Core.Extensions
{
    public static class ByteArrayExtensions
    {
        private static ThreadLocal<SHA256> _sha256 = new ThreadLocal<SHA256>(() => SHA256.Create());

        /// <summary>
        /// Generate SHA256 hash
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return SHA256 hash</returns>
        public static byte[] Sha256(this IEnumerable<byte> value)
        {
            return _sha256.Value.ComputeHash(value.ToArray());
        }

        /// <summary>
        /// Generate SHA256 hash
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="offset">Offset</param>
        /// <param name="count">Count</param>
        /// <returns>Return SHA256 hash</returns>
        public static byte[] Sha256(this byte[] value, int offset, int count)
        {
            return _sha256.Value.ComputeHash(value, offset, count);
        }

        public static string ToHexString(this IEnumerable<byte> value, bool append0x = false)
        {
            var sb = new StringBuilder();

            foreach (var b in value)
                sb.AppendFormat("{0:x2}", b);

            if (append0x)
            {
                if (sb.Length > 0) return "0x" + sb.ToString();
            }

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
