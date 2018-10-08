using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Types;

namespace NeoSharp.Core.Network
{
    public class PeerMessageListener : IPeerMessageListener
    {
        #region Private Fields
        private static readonly TimeSpan DefaultMessagePollingInterval = TimeSpan.FromMilliseconds(10);
        private const int MaxBlocksCountToSync = 500;
        private const int MaxParallelBlockRequestsForSync = 4;
        private static readonly TimeSpan DefaultBlockWaitingInterval = TimeSpan.FromMilliseconds(500);
        private static readonly TimeSpan DefaultBlockSynchronizingInterval = TimeSpan.FromMilliseconds(5000);

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

            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!peer.IsReady)
                    {
                        await _asyncDelayer.Delay(DefaultBlockWaitingInterval, cancellationToken);
                        continue;
                    }

                    var currentBlock = _blockchainContext.CurrentBlock;
                    var lastBlockHeader = _blockchainContext.LastBlockHeader;

                    if (currentBlock.Index <= lastBlockHeader.Index &&
                        lastBlockHeader.Index < peer.Version.CurrentBlockIndex)
                    {
                        await SynchronizeBlocks(peer, currentBlock.Index + 1, lastBlockHeader.Index);
                        await _asyncDelayer.Delay(DefaultBlockSynchronizingInterval, cancellationToken);
                    }
                    else
                    {
                        await _asyncDelayer.Delay(DefaultBlockWaitingInterval, cancellationToken);
                    }
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        #endregion

        private async Task SynchronizeBlocks(IPeer source, uint fromHeight, uint toHeight)
        {
            toHeight = Math.Min(fromHeight + MaxBlocksCountToSync * MaxParallelBlockRequestsForSync, toHeight);

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
