using System;
using System.Collections.Concurrent;
using System.Linq;
using NeoSharp.Core.Extensions;
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

            ValidateBlock(block);

            if (!_blockPool.TryAdd(block.Index, block))
            {
                throw new InvalidOperationException($"The block with index \"{block.Index}\" was already queued to be added.");
            }

            OnAdded?.Invoke(this, block);

            DiscardInvalidBlocks(block);
            PrioritizeBlocks();
        }

        public bool Contains(UInt256 blockHash)
        {
            if (blockHash == null) throw new ArgumentNullException(nameof(blockHash));
            if (blockHash == UInt256.Zero) throw new ArgumentException(nameof(blockHash));

            return _blockPool.Values.Any(b => b.Hash == blockHash);
        }

        public void Remove(uint index)
        {
            _blockPool.TryRemove(index, out _);
        }

        private void ValidateBlock(Block block)
        {
            if (CurrentBlock == null)
            {
                if (block.Index != 0)
                {
                    throw new InvalidOperationException("The current block is unknown. The genesis block can only be added.");
                }
            }
            else
            {
                if (!TryValidateBlock(block, GetPreviousBlock(block), out var errorMessage))
                {
                    throw new InvalidOperationException(errorMessage);
                }
            }
        }

        private Block GetPreviousBlock(Block block)
        {
            var prevBlockIndex = _blockPool.Keys
                .AsParallel()
                .OrderByDescending(_ => _)
                .Cast<uint?>()
                .FirstOrDefault(bi => block.Index > bi);

            if (prevBlockIndex == null ||
                !_blockPool.TryGetValue(prevBlockIndex.Value, out var prevBlock))
            {
                prevBlock = CurrentBlock;
            }

            return prevBlock;
        }

        private static bool TryValidateBlock(Block block, Block prevBlock, out string errorMessage)
        {
            if (block.Index <= prevBlock.Index)
            {
                errorMessage = $"The block with index \"{block.Index}\" is already added.";
                return false;
            }

            if (block.Timestamp <= prevBlock.Timestamp)
            {
                errorMessage = $"The block with index \"{block.Index}\" is outdated.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private void DiscardInvalidBlocks(Block block)
        {
            _blockPool.Keys
                .AsParallel()
                .OrderBy(_ => _)
                .SkipWhile(bi => block.Index >= bi || TryValidateBlock(block, GetPreviousBlock(block), out _))
                .ToArray()
                .ForEach(Remove);
        }

        private void PrioritizeBlocks()
        {
            if (Size < Capacity) return;

            _blockPool.Keys
                .AsParallel()
                .OrderByDescending(_ => _)
                .Take(Math.Max(Capacity - Size, 0))
                .ToArray()
                .ForEach(Remove);
        }
    }
}