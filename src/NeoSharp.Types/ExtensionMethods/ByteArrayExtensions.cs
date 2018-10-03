using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoSharp.Types.ExtensionMethods
{
    public static class ByteArrayExtensions
    {
        /// <summary>
        /// Convert Byte Array to Int32
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="startIndex">Start index</param>
        /// <returns>Int32</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ToInt32(this byte[] value, int startIndex = 0)
        {
            return BitConverter.ToInt32(value, startIndex);
        }

        /// <summary>
        /// Convert Byte Array to UInt32
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="startIndex">Start index</param>
        /// <returns>UInt32</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ToUInt32(this byte[] value, int startIndex = 0)
        {
            return BitConverter.ToUInt32(value, startIndex);
        }

        /// <summary>
        /// Convert to Hex String
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="append0X">Append 0x hex prefix</param>
        /// <returns>String</returns>
        public static string ToHexString(this IEnumerable<byte> value, bool append0X = false)
        {
            var sb = new StringBuilder();

            foreach (var b in value)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            if (append0X && sb.Length > 0)
            {
                return $"0x{sb}";
            }

            return sb.ToString();
        }
    }
}
