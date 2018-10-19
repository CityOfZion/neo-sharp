using System;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Network;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class BlockProcessor : IBlockProcessor
    {
        #region Private Fields 

        private static readonly TimeSpan DefaultBlockPollingInterval = TimeSpan.FromMilliseconds(100);

        private readonly IBlockPool _blockPool;
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly ISigner<Block> _blockSigner;
        private readonly IBlockPersister _blockPersister;
        private readonly IBlockchainContext _blockchainContext;
        private readonly IBroadcaster _broadcaster;
        private readonly ILogger<BlockProcessor> _logger;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        #endregion

        #region Constructor 

        public BlockProcessor(
            IBlockPool blockPool,
            IAsyncDelayer asyncDelayer,
            ISigner<Block> blockSigner,
            IBlockPersister blockPersister,
            IBlockchainContext blockchainContext,
            IBroadcaster broadcaster,
            ILogger<BlockProcessor> logger)
        {
            _blockPool = blockPool ?? throw new ArgumentNullException(nameof(blockPool));
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
            _blockSigner = blockSigner ?? throw new ArgumentNullException(nameof(blockSigner));
            _blockPersister = blockPersister ?? throw new ArgumentNullException(nameof(blockPersister));
            _blockchainContext = blockchainContext ?? throw new ArgumentNullException(nameof(blockchainContext));
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region IBlockProcessor implementation
        // TODO #384: We will read the current block from Blockchain
        // because the logic to get that too complicated 
        /// <inheritdoc />
        public void Run()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var block = _blockchainContext.CurrentBlock;
                    var nextBlockHeight = block?.Index + 1U ?? 0U;

                    //if (block != null && _blockchainContext.IsPeerConnected && _blockchainContext.NeedPeerSync && !_blockchainContext.IsSyncing)
                    //{
                    //    _broadcaster.Broadcast(new GetBlocksMessage(block.Hash));
                    //    _blockchainContext.IsSyncing = true;
                    //}

                    if (!_blockPool.TryGet(nextBlockHeight, out block))
                    {
                        await _asyncDelayer.Delay(DefaultBlockPollingInterval, cancellationToken);
                        continue;
                    }

                    await _blockPersister.Persist(block);

                    _blockPool.Remove(nextBlockHeight);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <inheritdoc />
        public async Task AddBlock(Block block)
        {
            if (block == null) throw new ArgumentNullException(nameof(block));

            var currentBlockHeight = _blockchainContext.CurrentBlock?.Index ?? 0U;

            if (currentBlockHeight >= block.Index && block.Index > currentBlockHeight + _blockPool.Size)
            {
                return;
            }

            if (block.Hash == null)
            {
                _blockSigner.Sign(block);
            }

            var blockHash = block.Hash;

            if (blockHash == null || blockHash == UInt256.Zero) throw new ArgumentException(nameof(blockHash));

            var blockExists = _blockPool.Contains(blockHash);
            if (blockExists)
            {
                _logger.LogWarning($"The block \"{blockHash.ToString(true)}\" was already queued to be added.");
                return;
            }

            if (!await _blockPersister.IsBlockPersisted(block))
            {
                _blockPool.Add(block);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        #endregion
    }
}