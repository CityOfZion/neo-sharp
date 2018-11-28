using System.Numerics;
using NeoSharp.Core.Exceptions;

namespace NeoSharp.Core.Extensions
{
    public static class BigIntegerExtensions
    {
        /// <summary>
        /// Get Bit Length
        /// </summary>
        /// <param name="i">Source</param>
        /// <returns>Length</returns>
        public static int GetBitLength(this BigInteger i)
        {
            var b = i.ToByteArray();
            return (b.Length - 1) * 8 + (i.Sign > 0 ? b[b.Length - 1] : 255 - b[b.Length - 1]).BitLen();
        }

        /// <summary>
        /// Get Lowest Set Bit
        /// </summary>
        /// <param name="i">Source</param>
        /// <returns>Lowest Set Bit</returns>
        public static int GetLowestSetBit(this BigInteger i)
        {
            if (i.Sign == 0)
                return -1;
            var b = i.ToByteArray();
            var w = 0;
            while (b[w] == 0)
                w++;
            for (var x = 0; x < 8; x++)
                if ((b[w] & 1 << x) > 0)
                    return x + w * 8;
            throw new InconsistentBitException();
        }

        public static BigInteger Mod(this BigInteger x, BigInteger y)
        {
            x %= y;
            if (x.Sign < 0)
                x += y;
            return x;
        }

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

        public static bool TestBit(this BigInteger i, int index)
        {
            return (i & (BigInteger.One << index)) > BigInteger.Zero;
        }
    }
}