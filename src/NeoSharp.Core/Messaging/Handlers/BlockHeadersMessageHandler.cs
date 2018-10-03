using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Types;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class BlockHeadersMessageHandler : MessageHandler<BlockHeadersMessage>
    {
        class PeerHandler
        {
            private readonly IPeer _sender;

            public PeerHandler(IPeer sender)
            {
                _sender = sender;
            }

            public async void HeadersPersisted(object sender, BlockHeader[] blockHeaders)
            {
                var blockHashes = blockHeaders
                    .Select(bh => bh.Hash)
                    .Where(bh => bh != null)
                    .ToArray();

                await SynchronizeBlocks(_sender, blockHashes);
            }
        }

        #region Private Fields 
        private const int MaxBlocksCountToSync = 500;

        private readonly IBlockPersister _blockPersister;
        private readonly IBlockchainContext _blockchainContext;
        private readonly ILogger<BlockHeadersMessageHandler> _logger;
        #endregion

        #region Constructor 

        public BlockHeadersMessageHandler
            (
            IBlockPersister blockPersister,
            IBlockchainContext blockchainContext,
            ILogger<BlockHeadersMessageHandler> logger
            )
        {
            _blockPersister = blockPersister ?? throw new ArgumentNullException(nameof(blockPersister));
            _blockchainContext = blockchainContext ?? throw new ArgumentNullException(nameof(blockchainContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region MessageHandler override Methods 

        /// <inheritdoc />
        public override async Task Handle(BlockHeadersMessage message, IPeer sender)
        {
            var h = new PeerHandler(sender);

            try
            {
                _blockPersister.OnBlockHeadersPersisted += h.HeadersPersisted;

                // If the remote node have Blocks, the flag Type will be Extended, and we want to store this as Headers

                message.Payload.Headers.ForEach(a => a.Type = HeaderType.Header);

                await _blockPersister.Persist(message.Payload.Headers ?? new BlockHeader[0]);
            }
            finally
            {
                _blockPersister.OnBlockHeadersPersisted -= h.HeadersPersisted;
            }

            if (_blockchainContext.LastBlockHeader.Index < sender.Version.CurrentBlockIndex)
            {
                _logger.LogInformation(
                    $"The peer has {sender.Version.CurrentBlockIndex + 1} blocks but the current number of block headers is {_blockchainContext.LastBlockHeader.Index + 1}.");
                await sender.Send(new GetBlockHeadersMessage(_blockchainContext.LastBlockHeader.Hash));
            }
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message) => message is BlockHeadersMessage;

        #endregion

        #region Private Methods 

        private static async Task SynchronizeBlocks(IPeer source, IReadOnlyCollection<UInt256> blockHashes)
        {
            var batchesCount = blockHashes.Count / MaxBlocksCountToSync + (blockHashes.Count % MaxBlocksCountToSync != 0 ? 1 : 0);

            for (var i = 0; i < batchesCount; i++)
            {
                var blockHashesInBatch = blockHashes
                    .Skip(i * MaxBlocksCountToSync)
                    .Take(MaxBlocksCountToSync);

                await source.Send(new GetDataMessage(InventoryType.Block, blockHashesInBatch));
            }
        }

        #endregion
    }
}