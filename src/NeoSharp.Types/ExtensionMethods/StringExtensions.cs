using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NeoSharp.Cryptography;

namespace NeoSharp.Types.ExtensionMethods
{
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a Address into a ScriptHash representation 
        /// </summary>
        /// <param name="address">Address</param>
        /// <returns>The script hash.</returns>
        public static UInt160 ToScriptHash(this string address)
        {
            var buffer = Crypto.Default.Base58CheckDecode(address);

            if (buffer.Length != 21 || buffer[0] != 0x017)
            {
                throw (new ArgumentException(nameof(address)));
            }
            else
            {
                return new UInt160(buffer.Skip(1).ToArray());
            }
        }

        /// <summary>
        /// Check string is in hex format.
        /// </summary>
        /// <returns>if hex in string was onlyed true, otherwise false.</returns>
        /// <param name="value">Value.</param>
        public static bool IsHexString(this string value)
        {
            return Regex.IsMatch(value, @"\A\b(0[xX])?[0-9a-fA-F]+\b\Z");
        }

        /// <summary>
        /// Convert Hex string to byte array
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="limit">Limit</param>
        /// <returns>Byte Array</returns>
        public static byte[] HexToBytes(this string value, int limit = 0)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new byte[0];
            }

            if (value.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
            {
                value = value.Substring(2);
            }

            if (value.Length % 2 == 1)
            {
                throw new FormatException();
            }

            if (limit != 0 && value.Length != limit)
            {
                throw new FormatException();
            }

            var result = new byte[value.Length / 2];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = byte.Parse(value.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
            }

            return result;
        }
    }
}
