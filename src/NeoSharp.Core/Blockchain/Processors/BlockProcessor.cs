using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Processors
{
    public class BlockProcessor : IBlockProcessor
    {
        private static readonly TimeSpan DefaultBlockPollingInterval = TimeSpan.FromMilliseconds(100);

        private readonly IRepository _repository;
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly IProcessor<Transaction> _transactionProcessor;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ConcurrentDictionary<uint, Block> _blockPool = new ConcurrentDictionary<uint, Block>();

        public Block CurrentBlock { get; private set; }

        public int BlocksInPoolCount => _blockPool.Count;

        public int MaxBlocksInPoolCount => 10_000;

        public event Func<Block, Task> OnBlockProcessed;

        public BlockProcessor(
            IRepository repository,
            IAsyncDelayer asyncDelayer,
            IProcessor<Transaction> transactionProcessor)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
            _transactionProcessor = transactionProcessor ?? throw new ArgumentNullException(nameof(transactionProcessor));
        }

        public void Run()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                while (cancellationToken.IsCancellationRequested)
                {
                    var blockIndex = CurrentBlock?.Index + 1 ?? 0;

                    if (_blockPool.TryGetValue(blockIndex, out var block) == false)
                    {
                        await _asyncDelayer.Delay(DefaultBlockPollingInterval, cancellationToken);
                        continue;
                    }

                    await Process(block);

                    _blockPool.Remove(blockIndex, out block);

                    CurrentBlock = block;
                    OnBlockProcessed?.Invoke(CurrentBlock);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public void AddBlock(Block block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            block.UpdateHash();

            if (CurrentBlock == null && block.Index != 0)
            {
                // TODO: For now let's assume it is the genesis block
                // but we need to go db to read the real current block
                throw new InvalidOperationException("The current block is unknown. The genesis block can only be added.");
            }

            if (CurrentBlock != null && block.Index <= CurrentBlock.Index)
            {
                throw new InvalidOperationException($"The block with index \"{block.Index}\" is already added.");
            }

            if (CurrentBlock != null && CurrentBlock.Timestamp >= block.Timestamp)
            {
                throw new InvalidOperationException($"The block with index \"{block.Index}\" is outdated.");
            }

            if (BlocksInPoolCount + 1 >= MaxBlocksInPoolCount)
            {
                throw new InvalidOperationException("The block pool contains max number of blocks.");
            }

            if (_blockPool.TryAdd(block.Index, block))
            {
                throw new InvalidOperationException($"The block with index \"{block.Index}\" was already queued to be added.");
            }
        }

        public async Task<bool> ContainsBlock(UInt256 blockHash)
        {
            if (blockHash == null) throw new ArgumentNullException(nameof(blockHash));
            if (blockHash == UInt256.Zero) throw new ArgumentException(nameof(blockHash));

            if (_blockPool.Values.Any(b => b.Hash == blockHash))
            {
                return true;
            }

            var header = await _repository.GetBlockHeader(blockHash);

            return header != null && header.Type == BlockHeader.HeaderType.Extended;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        protected async Task Process(Block block)
        {
            foreach (var tx in block.Transactions)
                await _transactionProcessor.Process(tx);

            await _repository.AddBlockHeader(block.GetBlockHeader());
            await _repository.SetTotalBlockHeight(block.Index);
        }
    }
}