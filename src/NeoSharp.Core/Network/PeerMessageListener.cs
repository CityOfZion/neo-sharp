using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Network
{
    public class PeerMessageListener : IPeerMessageListener
    {
        #region Private Fields
        private static readonly TimeSpan DefaultMessagePollingInterval = TimeSpan.FromMilliseconds(10);
        private const int MaxBlocksCountToSync = 500;
        private const int MaxParallelBlockRequestsForSync = 4;
        private static readonly TimeSpan DefaultBlockWaitingInterval = TimeSpan.FromMilliseconds(500);
        private static readonly TimeSpan DefaultBlockSynchronizingInterval = TimeSpan.FromMilliseconds(1_000);
        private static readonly TimeSpan DefaultPeerWaitingInterval = TimeSpan.FromMilliseconds(2_000);
        private static readonly TimeSpan DefaultPeerConnectingInterval = TimeSpan.FromMilliseconds(5_000);

        private readonly IAsyncDelayer _asyncDelayer;
        private readonly IMessageHandlerProxy _messageHandlerProxy;
        private readonly IServerContext _serverContext;
        private readonly IBlockchainContext _blockchainContext;
        private readonly IBlockRepository _blockRepository;

        #endregion

        #region Constructor

        public PeerMessageListener(
            IAsyncDelayer asyncDelayer,
            IMessageHandlerProxy messageHandlerProxy,
            IServerContext serverContext,
            IBlockchainContext blockchainContext,
            IBlockRepository blockRepository)
        {
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
            _messageHandlerProxy = messageHandlerProxy ?? throw new ArgumentNullException(nameof(messageHandlerProxy));
            _serverContext = serverContext ?? throw new ArgumentNullException(nameof(serverContext));
            _blockchainContext = blockchainContext ?? throw new ArgumentNullException(nameof(blockchainContext));
            _blockRepository = blockRepository ?? throw new ArgumentNullException(nameof(blockRepository));
        }

        #endregion

        #region IPeerMessageListener implementation 

        public void StartFor(IPeer peer, CancellationToken cancellationToken)
        {
            // Initiate handshake
            peer.Send(new VersionMessage(_serverContext.Version));

            // run main message listening loop
            Task.Factory.StartNew(async () =>
            {
                while (peer.IsConnected)
                {
                    var message = await peer.Receive();
                    if (message == null)
                    {
                        await _asyncDelayer.Delay(DefaultMessagePollingInterval, cancellationToken);
                        continue;
                    }

                    // TODO #369: Peer that sending wrong messages has to be disconnected.
                    if (peer.IsReady == message.IsHandshakeMessage()) continue;

                    await _messageHandlerProxy.Handle(message, peer);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            // run block synchronization loop
            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!peer.IsReady)
                    {
                        await _asyncDelayer.Delay(DefaultPeerWaitingInterval, cancellationToken);
                        continue;
                    }

                    var currentBlock = _blockchainContext.CurrentBlock;
                    var lastBlockHeader = _blockchainContext.LastBlockHeader;

                    if (currentBlock.Index <= lastBlockHeader.Index &&
                        lastBlockHeader.Index < peer.Version.CurrentBlockIndex)
                    {
                        await SynchronizeBlocks(peer, currentBlock.Index + 1, lastBlockHeader.Index);
                        await _asyncDelayer.Delay(DefaultBlockSynchronizingInterval * (new Random(Environment.TickCount)).Next(MaxParallelBlockRequestsForSync), cancellationToken);
                    }
                    else
                    {
                        await _asyncDelayer.Delay(DefaultBlockWaitingInterval, cancellationToken);
                    }
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            // run peer discovery loop
            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!peer.IsReady)
                    {
                        await _asyncDelayer.Delay(DefaultPeerWaitingInterval, cancellationToken);
                        continue;
                    }

                    if (_serverContext.ConnectedPeers.Count <= _serverContext.MaxConnectedPeers)
                    {
                        await peer.Send<GetAddrMessage>();
                    }

                    await _asyncDelayer.Delay(DefaultPeerConnectingInterval, cancellationToken);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        #endregion

        private async Task SynchronizeBlocks(IPeer source, uint fromHeight, uint toHeight)
        {
            if (fromHeight + MaxBlocksCountToSync * MaxParallelBlockRequestsForSync > toHeight)
                return;

            toHeight = fromHeight + MaxBlocksCountToSync * MaxParallelBlockRequestsForSync - 1;

            var blockHashes = await _blockRepository.GetBlockHashes(fromHeight, toHeight);

            if (blockHashes.Any())
            {
                var batchesCount = blockHashes.Count() / MaxBlocksCountToSync + (blockHashes.Count() % MaxBlocksCountToSync != 0 ? 1 : 0);

                for (var i = 0; i < batchesCount; i++)
                {
                    var blockHashesInBatch = blockHashes
                        .Skip(i * MaxBlocksCountToSync)
                        .Take(MaxBlocksCountToSync);

                    await source.Send(new GetDataMessage(InventoryType.Block, blockHashesInBatch));
                }
            }
        }
    }
}
