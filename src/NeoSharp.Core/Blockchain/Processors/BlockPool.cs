using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Processors
{
    public class BlockPool : IBlockPool
    {
        private const int DefaultCapacity = 10_000;

        private readonly ConcurrentDictionary<uint, Block> _blockPool = new ConcurrentDictionary<uint, Block>();

        public Block CurrentBlock { get; set; }

        public int Size => _blockPool.Count;

        public int Capacity => DefaultCapacity;

        public event EventHandler<Block> OnAdded;

        public bool TryGet(uint index, out Block block)
        {
            return _blockPool.TryGetValue(index, out block);
        }

        public void Add(Block block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            block.UpdateHash();

            if (CurrentBlock == null && block.Index != 0)
            {
                throw new InvalidOperationException("The current block is unknown. The genesis block can only be added.");
            }

            if (CurrentBlock != null && block.Index <= CurrentBlock.Index)
            {
                throw new InvalidOperationException($"The block with index \"{block.Index}\" is already added.");
            }

            if (CurrentBlock != null && block.Timestamp <= CurrentBlock.Timestamp)
            {
                throw new InvalidOperationException($"The block with index \"{block.Index}\" is outdated.");
            }

            if (Size + 1 >= Capacity)
            {
                throw new InvalidOperationException("The block pool contains max number of blocks.");
            }

            if (_blockPool.TryAdd(block.Index, block))
            {
                throw new InvalidOperationException($"The block with index \"{block.Index}\" was already queued to be added.");
            }

            OnAdded?.Invoke(this, block);
        }

        public bool Contains(UInt256 blockHash)
        {
            if (blockHash == null) throw new ArgumentNullException(nameof(blockHash));
            if (blockHash == UInt256.Zero) throw new ArgumentException(nameof(blockHash));

            return _blockPool.Values.Any(b => b.Hash == blockHash);
        }

        public void Remove(uint index)
        {
            ((IDictionary<uint, Block>)_blockPool).Remove(index);
        }
    }
}