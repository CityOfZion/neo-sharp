using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Split a command line into enumerable
        ///     https://stackoverflow.com/a/24829691
        /// </summary>
        /// <param name="commandLine">Command line</param>
        /// <returns>Return the ienumerable result</returns>
        public static IEnumerable<CommandToken> SplitCommandLine(this string commandLine)
        {
            var inQuotes = false;
            var isEscaping = false;

            var reslist = commandLine.Split((c) =>
             {
                 if (c == '\\' && !isEscaping) { isEscaping = true; return false; }

                 if (c == '\"' && !isEscaping)
                     inQuotes = !inQuotes;

                 isEscaping = false;

                 return !inQuotes && char.IsWhiteSpace(c)/*c == ' '*/;
             });

            foreach (var (Value, Index) in reslist)
            {
                var cmd = new CommandToken(Value, Index, Value.Length);

                if (string.IsNullOrEmpty(cmd.Value)) continue;

                yield return cmd;
            }
        }

        /// <summary>
        /// Split string into enumerable
        /// </summary>
        /// <param name="str">String</param>
        /// <param name="controller">Controller Func</param>
        /// <returns></returns>
        public static IEnumerable<(string Value, int Index)> Split(this string str, Func<char, bool> controller)
        {
            var nextPiece = 0;

            for (var c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return (Value: str.Substring(nextPiece, c - nextPiece), Index: nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return (Value: str.Substring(nextPiece), Index: nextPiece);
        }

        /// <summary>
        /// Remove quote from string
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="quote">Quote</param>
        /// <returns>String</returns>
        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) && (input[0] == quote) && (input[input.Length - 1] == quote))
            {
                return input.Substring(1, input.Length - 2);
            }

            return input;
        }

        /// <summary>
        /// Converts SecureString to byte array
        /// </summary>
        /// <param name="secureString">SecureString</param>
        /// <param name="encoding">Encoding</param>
        /// <returns>Byte Array</returns>
        public static byte[] ToByteArray(this SecureString secureString, Encoding encoding = null)
        {
            if (secureString == null)
            {
                throw new ArgumentNullException(nameof(secureString));
            }

            encoding = encoding ?? Encoding.UTF8;

            var unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);

                return encoding.GetBytes(Marshal.PtrToStringUni(unmanagedString));
            }
            finally
            {
                if (unmanagedString != IntPtr.Zero)
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
                }
            }
        }

    }
}