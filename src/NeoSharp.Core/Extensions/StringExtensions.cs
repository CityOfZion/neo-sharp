using System;
using System.Collections.Generic;
using System.Globalization;

namespace NeoSharp.Core.Extensions
{
    public static class StringExtensions
    {
        public static IEnumerable<string> Split(this string str, Func<char, bool> controller)
        {
            var nextPiece = 0;

            for (var c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }

        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) && (input[0] == quote) && (input[input.Length - 1] == quote))
            {
                return input.Substring(1, input.Length - 2);
            }

            return input;
        }

        public static byte[] HexToBytes(this string value, int limit = 0)
        {
            if (string.IsNullOrEmpty(value))
                return new byte[0];
            if (value.StartsWith("0x"))
                value = value.Substring(2);
            if (value.Length % 2 == 1)
                throw new FormatException();
            if (limit != 0 && value.Length != limit)
                throw new FormatException();
            var result = new byte[value.Length / 2];
            for (var i = 0; i < result.Length; i++)
                result[i] = byte.Parse(value.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier);
            return result;
        }
    }
}