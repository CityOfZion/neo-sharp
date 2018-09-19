using System;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class BlockProcessor : IBlockProcessor
    {
        private static readonly TimeSpan DefaultBlockPollingInterval = TimeSpan.FromMilliseconds(100);

        private readonly IBlockPool _blockPool;
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly IBlockOperationsManager _blockOperationsManager;
        private readonly IBlockPersister _blockPersister;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Block _currentBlock;

        public event EventHandler<Block> OnBlockProcessed;

        public BlockProcessor(
            IBlockPool blockPool,
            IAsyncDelayer asyncDelayer,
            IBlockOperationsManager blockOperationsManager,
            IBlockPersister blockPersister)
        {
            _blockPool = blockPool ?? throw new ArgumentNullException(nameof(blockPool));
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
            _blockOperationsManager = blockOperationsManager;
            _blockPersister = blockPersister ?? throw new ArgumentNullException(nameof(_blockPersister));
        }

        // TODO #384: We will read the current block from Blockchain
        // because the logic to get that too complicated 
        public void Run(Block currentBlock)
        {
            _currentBlock = currentBlock;

            var cancellationToken = _cancellationTokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var nextBlockHeight = _currentBlock?.Index + 1 ?? 0;

                    if (!_blockPool.TryGet(nextBlockHeight, out var block))
                    {
                        await _asyncDelayer.Delay(DefaultBlockPollingInterval, cancellationToken);
                        continue;
                    }

                    await this._blockPersister.Persist(block);

                    _blockPool.Remove(nextBlockHeight);
                    _currentBlock = block;


                    OnBlockProcessed?.Invoke(this, block);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task AddBlock(Block block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            if (block.Hash == null)
            {
                _blockOperationsManager.Sign(block);
            }

            var blockHash = block.Hash;

            if (blockHash == null || blockHash == UInt256.Zero) throw new ArgumentException(nameof(blockHash));

            var blockExists = _blockPool.Contains(blockHash);
            if (blockExists)
            {
                throw new InvalidOperationException($"The block \"{blockHash.ToString(true)}\" was already queued to be added.");
            }

            if (!await this._blockPersister.IsBlockPersisted(block))
            {
                _blockPool.Add(block);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}