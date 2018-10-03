using System;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Network;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class BlockProcessor : IBlockProcessor
    {
        private static readonly TimeSpan DefaultBlockPollingInterval = TimeSpan.FromMilliseconds(100);

        private readonly IBlockPool _blockPool;
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly ISigner<Block> _blockSigner;
        private readonly IBlockPersister _blockPersister;
        private readonly IBlockchainContext _blockchainContext;
        private readonly IBroadcaster _broadcaster;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public event EventHandler<Block> OnBlockProcessed;

        public BlockProcessor(
            IBlockPool blockPool,
            IAsyncDelayer asyncDelayer,
            ISigner<Block> blockSigner,
            IBlockPersister blockPersister, 
            IBlockchainContext blockchainContext, 
            IBroadcaster broadcaster)
        {
            _blockPool = blockPool ?? throw new ArgumentNullException(nameof(blockPool));
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
            _blockSigner = blockSigner ?? throw new ArgumentNullException(nameof(blockSigner));
            _blockPersister = blockPersister ?? throw new ArgumentNullException(nameof(blockPersister));
            _blockchainContext = blockchainContext ?? throw new ArgumentNullException(nameof(blockchainContext));
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
        }

        // TODO #384: We will read the current block from Blockchain
        // because the logic to get that too complicated 
        public void Run(Block currentBlock)
        {
            _blockchainContext.CurrentBlock = currentBlock;

            var cancellationToken = _cancellationTokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (_blockchainContext.IsPeerConnected && _blockchainContext.NeedPeerSync && !_blockchainContext.IsSyncing)
                    {
                        _broadcaster.Broadcast(new GetBlocksMessage(_blockchainContext.CurrentBlock.Hash));
                        _blockchainContext.IsSyncing = true;
                    }

                    var nextBlockHeight = this._blockchainContext.CurrentBlock?.Index + 1 ?? 0;

                    if (!_blockPool.TryGet(nextBlockHeight, out var block))
                    {
                        await _asyncDelayer.Delay(DefaultBlockPollingInterval, cancellationToken);
                        continue;
                    }

                    await _blockPersister.Persist(block);

                    _blockPool.Remove(nextBlockHeight);
                    _blockchainContext.CurrentBlock = block;


                    OnBlockProcessed?.Invoke(this, block);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task AddBlock(Block block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            if (block.Hash == null)
            {
                _blockSigner.Sign(block);
            }

            var blockHash = block.Hash;

            if (blockHash == null || blockHash == UInt256.Zero) throw new ArgumentException(nameof(blockHash));

            var blockExists = _blockPool.Contains(blockHash);
            if (blockExists)
            {
                throw new InvalidOperationException($"The block \"{blockHash.ToString(true)}\" was already queued to be added.");
            }

            if (!await _blockPersister.IsBlockPersisted(block))
            {
                _blockPool.Add(block);
            }
        }

        /// <summary>
        /// Free resources
        /// </summary>
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}