using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Cryptography
{
    sealed class Murmur3 : HashAlgorithm
    {
        private const uint _c1 = 0xcc9e2d51;
        private const uint _c2 = 0x1b873593;
        private const int _r1 = 15;
        private const int _r2 = 13;
        private const uint _m = 5;
        private const uint _n = 0xe6546b64;

        private readonly uint _seed;
        private uint _hash;
        private int _length;

        public override int HashSize => 32;

        public Murmur3(uint seed)
        {
            _seed = seed;
            Initialize();
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _length += cbSize;
            int remainder = cbSize & 3;
            int alignedLength = ibStart + (cbSize - remainder);
            for (int i = ibStart; i < alignedLength; i += 4)
            {
                uint k = array.ToUInt32(i);
                k *= _c1;
                k = RotateLeft(k, _r1);
                k *= _c2;
                _hash ^= k;
                _hash = RotateLeft(_hash, _r2);
                _hash = _hash * _m + _n;
            }
            if (remainder > 0)
            {
                uint remainingBytes = 0;
                switch (remainder)
                {
                    case 3: remainingBytes ^= (uint)array[alignedLength + 2] << 16; goto case 2;
                    case 2: remainingBytes ^= (uint)array[alignedLength + 1] << 8; goto case 1;
                    case 1: remainingBytes ^= array[alignedLength]; break;
                }
                remainingBytes *= _c1;
                remainingBytes = RotateLeft(remainingBytes, _r1);
                remainingBytes *= _c2;
                _hash ^= remainingBytes;
            }
        }

        protected override byte[] HashFinal()
        {
            _hash ^= (uint)_length;
            _hash ^= _hash >> 16;
            _hash *= 0x85ebca6b;
            _hash ^= _hash >> 13;
            _hash *= 0xc2b2ae35;
            _hash ^= _hash >> 16;
            return BitConverter.GetBytes(_hash);
        }

        public override void Initialize()
        {
            _hash = _seed;
            _length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint RotateLeft(uint x, byte n)
        {
            return (x << n) | (x >> (32 - n));
        }
    }
}
