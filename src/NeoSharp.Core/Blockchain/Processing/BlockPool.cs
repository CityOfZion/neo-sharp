using System;
using System.Collections.Concurrent;
using System.Linq;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class BlockPool : IBlockPool
    {
        private const int DefaultCapacity = 10_000;

        private readonly ILogger<BlockPool> _logger;
        private readonly ConcurrentDictionary<uint, Block> _blockPool = new ConcurrentDictionary<uint, Block>();

        public int Size => _blockPool.Count;

        public int Capacity => DefaultCapacity;

        public event EventHandler<Block> OnAdded;

        public BlockPool(ILogger<BlockPool> logger)
        {
            _logger = logger;
        }

        public bool TryGet(uint height, out Block block)
        {
            return _blockPool.TryGetValue(height, out block);
        }

        public void Add(Block block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));
            if (block.Hash == null) throw new ArgumentException(nameof(block.Hash));
            if (block.Hash == UInt256.Zero) throw new ArgumentException(nameof(block.Hash));

            if (!_blockPool.TryAdd(block.Index, block))
            {
                throw new InvalidOperationException($"The block with height \"{block.Index}\" was already queued to be added.");
            }

            this._logger.LogInformation($"BlockPool count: {this._blockPool.Count}");
            OnAdded?.Invoke(this, block);

            PrioritizeBlocks();
        }

        public bool Contains(UInt256 hash)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            if (hash == UInt256.Zero) throw new ArgumentException(nameof(hash));

            return _blockPool.Values.Any(b => b.Hash == hash);
        }

        public void Remove(uint height)
        {
            _blockPool.TryRemove(height, out _);
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