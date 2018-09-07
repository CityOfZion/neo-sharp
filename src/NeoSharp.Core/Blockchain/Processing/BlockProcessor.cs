using System;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class BlockProcessor : IBlockProcessor
    {
        private static readonly TimeSpan DefaultBlockPollingInterval = TimeSpan.FromMilliseconds(100);

        private readonly IBlockPool _blockPool;
        private readonly ITransactionPool _transactionPool;
        private readonly IRepository _repository;
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly IBlockHeaderPersister _blockHeaderPersister;
        private readonly ITransactionPersister<Transaction> _transactionPersister;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Block _currentBlock;

        public event EventHandler<Block> OnBlockProcessed;

        public BlockProcessor(
            IBlockPool blockPool,
            ITransactionPool transactionPool,
            IRepository repository,
            IAsyncDelayer asyncDelayer,
            IBlockHeaderPersister blockHeaderPersister,
            ITransactionPersister<Transaction> transactionPersister)
        {
            _blockPool = blockPool ?? throw new ArgumentNullException(nameof(blockPool));
            _transactionPool = transactionPool ?? throw new ArgumentNullException(nameof(transactionPool));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
            _blockHeaderPersister = blockHeaderPersister ?? throw new ArgumentNullException(nameof(blockHeaderPersister));
            _transactionPersister = transactionPersister ?? throw new ArgumentNullException(nameof(transactionPersister));
        }

        // TODO: We will read the current block from Blockchain
        // because the logic to get that too complicated 
        public void Run(Block currentBlock)
        {
            _currentBlock = currentBlock ?? throw new ArgumentNullException(nameof(currentBlock));

            var cancellationToken = _cancellationTokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var nextBlockHeight = _currentBlock.Index + 1;

                    if (!_blockPool.TryGet(nextBlockHeight, out var block))
                    {
                        await _asyncDelayer.Delay(DefaultBlockPollingInterval, cancellationToken);
                        continue;
                    }

                    await Persist(block);

                    _blockPool.Remove(nextBlockHeight);
                    _currentBlock = block;


                    OnBlockProcessed?.Invoke(this, block);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task AddBlock(Block block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            block.UpdateHash();

            var blockHash = block.Hash;

            if (blockHash == UInt256.Zero) throw new ArgumentException(nameof(blockHash));

            var blockExists = _blockPool.Contains(blockHash);
            if (blockExists)
            {
                throw new InvalidOperationException($"The block \"{blockHash.ToString(true)}\" was already queued to be added.");
            }

            var blockHeader = await _repository.GetBlockHeader(blockHash);
            if (blockHeader?.Type == BlockHeader.HeaderType.Extended)
            {
                throw new InvalidOperationException($"The block \"{blockHash.ToString(true)}\" exists already on the blockchain.");
            }

            if (blockHeader != null && blockHeader.Hash != block.Hash)
            {
                throw new InvalidOperationException($"The block \"{blockHash.ToString(true)}\" has an invalid hash.");
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
            var blockHeader = await _repository.GetBlockHeader(block.Hash);
            if (blockHeader == null)
            {
                await _blockHeaderPersister.Persist(block.GetBlockHeader());
            }

            foreach (var transaction in block.Transactions)
            {
                await _transactionPersister.Persist(transaction);
                _transactionPool.Remove(transaction.Hash);
            }
        }
    }
}