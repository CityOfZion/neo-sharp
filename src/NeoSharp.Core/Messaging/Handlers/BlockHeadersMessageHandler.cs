using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class BlockHeadersMessageHandler : IMessageHandler<BlockHeadersMessage>
    {
        private const int MaxBlocksCountToSync = 500;

        private readonly IBlockHeaderPersister _blockHeaderPersister;
        private readonly IBlockchain _blockchain;
        private readonly ILogger<BlockHeadersMessageHandler> _logger;

        public BlockHeadersMessageHandler(IBlockHeaderPersister blockHeaderPersister, IBlockchain blockchain, ILogger<BlockHeadersMessageHandler> logger)
        {
            _blockHeaderPersister = blockHeaderPersister ?? throw new ArgumentNullException(nameof(blockHeaderPersister));
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(BlockHeadersMessage message, IPeer sender)
        {
            EventHandler<BlockHeader[]> blockHeadersPersisted =
                async (_, blockHeaders) => await BlockHeadersPersisted(sender, blockHeaders);

            try
            {
                _blockHeaderPersister.OnBlockHeadersPersisted += blockHeadersPersisted;

                await _blockHeaderPersister.Persist(message.Payload.Headers ?? new BlockHeader[0]);
            }
            finally
            {
                _blockHeaderPersister.OnBlockHeadersPersisted -= blockHeadersPersisted;
            }

            if (_blockchain.LastBlockHeader.Index < sender.Version.CurrentBlockIndex)
            {
                _logger.LogInformation(
                    $"The peer has {sender.Version.CurrentBlockIndex + 1} blocks but the current number of block headers is {_blockchain.LastBlockHeader.Index + 1}.");
                await sender.Send(new GetBlockHeadersMessage(_blockchain.LastBlockHeader.Hash));
            }
        }

        #region Find a better place for block sync

        private static async Task BlockHeadersPersisted(IPeer source, IEnumerable<BlockHeader> blockHeaders)
        {
            var blockHashes = blockHeaders
                .Select(bh => bh.Hash)
                .Where(bh => bh != null)
                .ToArray();

            await SynchronizeBlocks(source, blockHashes);
        }

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