using System.Collections;
using System.Linq;
using NeoSharp.Cryptography;

namespace NeoSharp.Core.Cryptography
{
    public class BloomFilter
    {
        private readonly uint[] _seeds;
        private readonly BitArray _bits;

        public int K => _seeds.Length;
        public int M => _bits.Length;
        public readonly uint Tweak;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="m">Size</param>
        /// <param name="k">Hash iterations</param>
        /// <param name="nTweak">Seed</param>
        /// <param name="elements">Initial elements</param>
        public BloomFilter(int m, int k, uint nTweak, byte[] elements = null)
        {
            _seeds = Enumerable.Range(0, k).Select(p => (uint)p * 0xFBA4C795 + nTweak).ToArray();
            _bits = elements == null ? new BitArray(m) : new BitArray(elements);
            _bits.Length = m;
            Tweak = nTweak;
        }

        /// <summary>
        /// Add element to structure
        /// </summary>
        /// <param name="element">Element</param>
        public void Add(byte[] element)
        {
            foreach (var i in _seeds.AsParallel().Select(s => Crypto.Default.Murmur32(element, s)))
                _bits.Set((int)(i % (uint)_bits.Length), true);
        }

        /// <summary>
        /// Check element in structure
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>If probably present</returns>
        public bool Check(byte[] element)
        {
            foreach (var i in _seeds.AsParallel().Select(s => Crypto.Default.Murmur32(element, s)))
                if (!_bits.Get((int)(i % (uint)_bits.Length)))
                    return false;
            return true;
        }

        /// <summary>
        /// BloomFilter bit structure
        /// </summary>
        /// <param name="newBits">Bytearray to store structure</param>
        public void GetBits(byte[] newBits)
        {
            _bits.CopyTo(newBits, 0);
        }
    }
}