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
        private readonly ITransactionPool _transactionPool;
        private readonly IRepository _repository;
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly ITransactionPersister<Transaction> _transactionPersister;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public event EventHandler<Block> OnBlockProcessed;

        public BlockProcessor(
            IBlockPool blockPool,
            ITransactionPool transactionPool,
            IRepository repository,
            IAsyncDelayer asyncDelayer,
            ITransactionPersister<Transaction> transactionPersister)
        {
            _blockPool = blockPool ?? throw new ArgumentNullException(nameof(blockPool));
            _transactionPool = transactionPool ?? throw new ArgumentNullException(nameof(transactionPool));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
            _transactionPersister = transactionPersister ?? throw new ArgumentNullException(nameof(transactionPersister));
        }

        // TODO: We will read the current block from Blockchain
        // because the logic to get that too complicated 
        public void Run(Block currentBlock)
        {
            _blockPool.CurrentBlock = currentBlock;

            var cancellationToken = _cancellationTokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var blockIndex = _blockPool.CurrentBlock?.Index + 1 ?? 0;

                    if (!_blockPool.TryGet(blockIndex, out var block))
                    {
                        await _asyncDelayer.Delay(DefaultBlockPollingInterval, cancellationToken);
                        continue;
                    }

                    await Persist(block);

                    _blockPool.Remove(blockIndex);
                    _blockPool.CurrentBlock = block;

                    OnBlockProcessed?.Invoke(this, block);
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

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        protected virtual async Task Persist(Block block)
        {
            foreach (var transaction in block.Transactions)
            {
                await _transactionPersister.Persist(transaction);
                _transactionPool.Remove(transaction.Hash);
            }

            await _repository.AddBlockHeader(block.GetBlockHeader());
            await _repository.SetTotalBlockHeight(block.Index);
        }

        private async Task<bool> ContainsBlock(UInt256 blockHash)
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
    }
}