using System;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NeoSharp.Core.Cryptography
{
    public static class Base58
    {
        /// <summary>
        /// base58 Alphabet
        /// </summary>
        public const string Alphabet = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";
        /// <summary>
        /// Base
        /// </summary>
        private static readonly BigInteger _base = new BigInteger(58);

        /// <summary>
        /// Decode
        /// </summary>
        /// <param name="input">String to be decoded</param>
        /// <returns>Decoded Byte Array</returns>
        public static byte[] Decode(string input)
        {
            BigInteger bi = BigInteger.Zero;
            for (int i = input.Length - 1; i >= 0; i--)
            {
                int index = Alphabet.IndexOf(input[i]);
                if (index == -1)
                    throw new FormatException();
                bi += index * BigInteger.Pow(_base, input.Length - 1 - i);
            }
            byte[] bytes = bi.ToByteArray();
            Array.Reverse(bytes);
            bool stripSignByte = bytes.Length > 1 && bytes[0] == 0 && bytes[1] >= 0x80;
            int leadingZeros = 0;
            for (int i = 0; i < input.Length && input[i] == Alphabet[0]; i++)
            {
                leadingZeros++;
            }
            byte[] tmp = new byte[bytes.Length - (stripSignByte ? 1 : 0) + leadingZeros];
            Array.Copy(bytes, stripSignByte ? 1 : 0, tmp, leadingZeros, tmp.Length - leadingZeros);
            return tmp;
        }

        /// <summary>
        /// Encode
        /// </summary>
        /// <param name="input">Byte Array to encode</param>
        /// <returns>Encoded string</returns>
        public static string Encode(byte[] input)
        {
            BigInteger value = new BigInteger(new byte[1].Concat(input).Reverse().ToArray());
            StringBuilder sb = new StringBuilder();
            while (value >= _base)
            {
                BigInteger mod = value % _base;
                sb.Insert(0, Alphabet[(int)mod]);
                value /= _base;
            }
            sb.Insert(0, Alphabet[(int)value]);
            foreach (byte b in input)
            {
                if (b == 0)
                    sb.Insert(0, Alphabet[0]);
                else
                    break;
            }
            return sb.ToString();
        }
    }
}