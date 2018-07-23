using System;
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

        private readonly IBlockPool _blockPool;
        private readonly IRepository _repository;
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly IProcessor<Transaction> _transactionProcessor;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public int BlockPoolSize => _blockPool.Size;

        public int BlockPoolCapacity => _blockPool.Capacity;

        public event Func<Block, Task> OnBlockProcessed;

        public BlockProcessor(
            IBlockPool blockPool,
            IRepository repository,
            IAsyncDelayer asyncDelayer,
            IProcessor<Transaction> transactionProcessor)
        {
            _blockPool = blockPool ?? throw new ArgumentNullException(nameof(blockPool));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
            _transactionProcessor = transactionProcessor ?? throw new ArgumentNullException(nameof(transactionProcessor));
        }

        // TODO: We will read the current block from Blockchain
        // because the logic to get that too complicated 
        public void Run(Block currentBlock)
        {
            _blockPool.CurrentBlock = currentBlock;

            var cancellationToken = _cancellationTokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                while (cancellationToken.IsCancellationRequested)
                {
                    var blockIndex = _blockPool.CurrentBlock?.Index + 1 ?? 0;

                    if (_blockPool.TryGet(blockIndex, out var block) == false)
                    {
                        await _asyncDelayer.Delay(DefaultBlockPollingInterval, cancellationToken);
                        continue;
                    }

                    await Process(block);

                    _blockPool.Remove(blockIndex);
                    _blockPool.CurrentBlock = block;

                    OnBlockProcessed?.Invoke(block);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task AddBlock(Block block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            var blockExists = await ContainsBlock(block.Hash);
            if (blockExists)
            {
                throw new InvalidOperationException($"The block \"{block.Hash.ToString(true)}\" exists already on the blockchain.");
            }

            _blockPool.Add(block);
        }

        public async Task<bool> ContainsBlock(UInt256 blockHash)
        {
            if (blockHash == null) throw new ArgumentNullException(nameof(blockHash));
            if (blockHash == UInt256.Zero) throw new ArgumentException(nameof(blockHash));

            var blockExists = _blockPool.Contains(blockHash);
            if (blockExists)
            {
                return true;
            }

            var blockHeader = await _repository.GetBlockHeader(blockHash);

            return blockHeader != null && blockHeader.Type == BlockHeader.HeaderType.Extended;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        protected virtual async Task Process(Block block)
        {
            foreach (var tx in block.Transactions)
                await _transactionProcessor.Process(tx);

            await _repository.AddBlockHeader(block.GetBlockHeader());
            await _repository.SetTotalBlockHeight(block.Index);
        }
    }
}