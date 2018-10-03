using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace NeoSharp.Cryptography
{
    internal sealed class Murmur3 : HashAlgorithm
    {
        private const uint C1 = 0xcc9e2d51;
        private const uint C2 = 0x1b873593;
        private const int R1 = 15;
        private const int R2 = 13;
        private const uint M = 5;
        private const uint N = 0xe6546b64;

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
                uint k = BitConverter.ToUInt32(array, i);
                k *= C1;
                k = RotateLeft(k, R1);
                k *= C2;
                _hash ^= k;
                _hash = RotateLeft(_hash, R2);
                _hash = _hash * M + N;
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
                remainingBytes *= C1;
                remainingBytes = RotateLeft(remainingBytes, R1);
                remainingBytes *= C2;
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
