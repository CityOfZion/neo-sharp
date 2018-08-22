using System;
using System.Globalization;

namespace NeoSharp.VM.Extensions
{
    public unsafe static class ByteArrayExtensions
    {
        /// <summary>
        /// Convert buffer to hex string
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return hex string</returns>
        public static string ToHexString(this byte[] data)
        {
            if (data == null) return "";

            var m = data.Length;
            if (m == 0) return "";

            var sb = new char[(m * 2) + 2];

            sb[0] = '0';
            sb[1] = 'x';

            for (int x = 0, y = 2; x < m; x++, y += 2)
            {
                 var hex = data[x].ToString("x2");

                sb[y] = hex[0];
                sb[y + 1] = hex[1];
            }

            return new string(sb);
        }
        /// <summary>
        /// Convert string in Hex format to byte array
        /// </summary>
        /// <param name="value">Hexadecimal string</param>
        /// <returns>Return byte array</returns>
        public static byte[] FromHexString(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return new byte[0];
            if (value.StartsWith("0x"))
                value = value.Substring(2);
            if (value.Length % 2 == 1)
                throw new FormatException();

            var result = new byte[value.Length / 2];
            for (var i = 0; i < result.Length; i++)
                result[i] = byte.Parse(value.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);

            return result;
        }
        /// <summary>
        /// Get bytes
        /// </summary>
        /// <param name="value">Value</param>
        public static byte[] GetBytes(this int value)
        {
            var bytes = new byte[4];

            fixed (byte* b = bytes)
                *((int*)b) = value;

            if (!BitConverter.IsLittleEndian)
            {
                // Convert to Little Endian

                Array.Reverse(bytes);
            }

            return bytes;
        }

        /// <summary>
        /// Get bytes
        /// </summary>
        /// <param name="value">Value</param>
        public static byte[] GetBytes(this ushort value)
        {
            var bytes = new byte[2];

            fixed (byte* b = bytes)
                *(ushort*)b = value;

            if (!BitConverter.IsLittleEndian)
            {
                // Convert to Little Endian

                Array.Reverse(bytes);
            }

            return bytes;
        }
        /// <summary>
        /// To Int32
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="startIndex">index</param>
        public static int ToInt32(this byte[] value, int startIndex)
        {
            if (!BitConverter.IsLittleEndian)
            {
                // Convert to Little Endian

                Array.Reverse(value, startIndex, 4);
            }

            fixed (byte* pbyte = &value[startIndex])
            {
                if (startIndex % 4 == 0)
                {
                    // data is aligned 
                    return *(int*)pbyte;
                }

                return (*pbyte) | (*(pbyte + 1) << 8) | (*(pbyte + 2) << 16) | (*(pbyte + 3) << 24);
            }
        }

        /// <summary>
        /// Return true if is ASCCI string
        /// </summary>
        /// <param name="data">Data</param>
        /// <returns>Return true if is ASCCI string</returns>
        public static bool IsASCIIPrintable(this byte[] data)
        {
            if (data == null) return false;

            foreach (var c in data)
            {
                if (c < 32 || c > 126)
                {
                    return false;
                }
            }

            return true;
        }
    }
}