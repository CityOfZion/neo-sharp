using System;
using System.Numerics;
using System.Security.Cryptography;

namespace NeoSharp.Core.Extensions
{
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// Mod
        /// </summary>
        /// <param name="x">Source</param>
        /// <param name="y">Operand</param>
        /// <returns>Result</returns>
        public static BigInteger Mod(this BigInteger x, BigInteger y)
        {
            x %= y;
            if (x.Sign < 0)
                x += y;
            return x;
        }

        /// <summary>
        /// Mod Inverse
        /// </summary>
        /// <param name="a">Source</param>
        /// <param name="n">Operand</param>
        /// <returns>Result</returns>
        public static BigInteger ModInverse(this BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0) v = (v + n) % n;
            return v;
        }


        public static BigInteger NextBigInteger(this Random rand, int sizeInBits)
        {
            // TODO: check accuracy and ut

            if (sizeInBits < 0) throw new ArgumentOutOfRangeException(nameof(sizeInBits));
            if (sizeInBits == 0)
                return 0;
            var b = new byte[sizeInBits / 8 + 1];
            rand.NextBytes(b);
            if (sizeInBits % 8 == 0)
                b[b.Length - 1] = 0;
            else
                b[b.Length - 1] &= (byte)((1 << sizeInBits % 8) - 1);
            return new BigInteger(b);
        }

        public static BigInteger NextBigInteger(this RandomNumberGenerator rng, int sizeInBits)
        {
            // TODO: check accuracy and ut

            if (sizeInBits < 0) throw new ArgumentOutOfRangeException(nameof(sizeInBits));
            if (sizeInBits == 0)
                return 0;
            var b = new byte[sizeInBits / 8 + 1];
            rng.GetBytes(b);
            if (sizeInBits % 8 == 0)
                b[b.Length - 1] = 0;
            else
                b[b.Length - 1] &= (byte)((1 << sizeInBits % 8) - 1);
            return new BigInteger(b);
        }

        public static bool TestBit(this BigInteger i, int index)
        {
            return (i & (BigInteger.One << index)) > BigInteger.Zero;
        }
    }
}
