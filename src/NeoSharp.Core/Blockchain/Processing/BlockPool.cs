using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class BlockPool : IBlockPool
    {
        private const int DefaultCapacity = 10_000;
        private readonly ConcurrentDictionary<uint, Block> _blockPool = new ConcurrentDictionary<uint, Block>();

        public int Size => _blockPool.Count;
        public int Capacity => DefaultCapacity;

        public bool TryGet(uint height, out Block block)
        {
            return _blockPool.TryGetValue(height, out block);
        }

        public bool TryAdd(Block block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));
            if (block.Hash == null) throw new ArgumentException(nameof(block.Hash));
            if (block.Hash == UInt256.Zero) throw new ArgumentException(nameof(block.Hash));

            if (!_blockPool.TryAdd(block.Index, block))
            {
                return false;
            }

            PrioritizeBlocks();


            return _blockPool.ContainsKey(block.Index);
        }

        public bool TryRemove(uint height)
        {
            return _blockPool.TryRemove(height, out _);
        }

        private void PrioritizeBlocks()
        {
            if (Size < Capacity) return;

            _blockPool.Keys
                .AsParallel()
                .OrderByDescending(_ => _)
                .Take(Math.Max(Size - Capacity, 0))
                .ToArray()
                .ForEach(h => TryRemove(h));
        }

        public IEnumerator<Block> GetEnumerator()
        {
            return _blockPool.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}