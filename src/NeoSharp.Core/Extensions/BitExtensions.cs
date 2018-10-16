using System;
using System.Numerics;
using NeoSharp.Core.Exceptions;

namespace NeoSharp.Core.Extensions
{
    public static class BitExtensions
    {
        /// <summary>
        /// Bit Length
        /// </summary>
        /// <param name="w">Source</param>
        /// <returns>Length</returns>
        public static int BitLen(this int w)
        {
            return (w < 1 << 15 ? (w < 1 << 7
                ? (w < 1 << 3 ? (w < 1 << 1
                ? (w < 1 << 0 ? (w < 0 ? 32 : 0) : 1)
                : (w < 1 << 2 ? 2 : 3)) : (w < 1 << 5
                ? (w < 1 << 4 ? 4 : 5)
                : (w < 1 << 6 ? 6 : 7)))
                : (w < 1 << 11
                ? (w < 1 << 9 ? (w < 1 << 8 ? 8 : 9) : (w < 1 << 10 ? 10 : 11))
                : (w < 1 << 13 ? (w < 1 << 12 ? 12 : 13) : (w < 1 << 14 ? 14 : 15)))) : (w < 1 << 23 ? (w < 1 << 19
                ? (w < 1 << 17 ? (w < 1 << 16 ? 16 : 17) : (w < 1 << 18 ? 18 : 19))
                : (w < 1 << 21 ? (w < 1 << 20 ? 20 : 21) : (w < 1 << 22 ? 22 : 23))) : (w < 1 << 27
                ? (w < 1 << 25 ? (w < 1 << 24 ? 24 : 25) : (w < 1 << 26 ? 26 : 27))
                : (w < 1 << 29 ? (w < 1 << 28 ? 28 : 29) : (w < 1 << 30 ? 30 : 31)))));
        }

        /// <summary>
        /// Get Bit Length
        /// </summary>
        /// <param name="i">Source</param>
        /// <returns>Length</returns>
        public static int GetBitLength(this BigInteger i)
        {
            var b = i.ToByteArray();
            return (b.Length - 1) * 8 + BitLen(i.Sign > 0 ? b[b.Length - 1] : 255 - b[b.Length - 1]);
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
    }
}