using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Extensions;
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
        private readonly IBlockchain _blockchain;
        private readonly ILogger<BlockHeadersMessageHandler> _logger;

        public BlockHeadersMessageHandler(IBlockchain blockchain, ILogger<BlockHeadersMessageHandler> logger)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(BlockHeadersMessage message, IPeer sender)
        {
            var lastBlockHeaderIndex = _blockchain.LastBlockHeader.Index;
            var missingBlockHeaders = (message.Payload.Headers ?? new BlockHeader[0])
                .Where(h => h != null && h.Index > lastBlockHeaderIndex)
                .Distinct(h => h.Index)
                .ToList();

            if (missingBlockHeaders.Count == 0) return;

            await _blockchain.AddBlockHeaders(missingBlockHeaders);

            // TODO: Find a better place for block sync

            var missingBlockHashes = missingBlockHeaders
                .Select(bh => bh.Hash)
                .Where(bh => bh != null)
                .ToList();

            await SynchronizeBlocks(missingBlockHashes, sender);

            if (_blockchain.LastBlockHeader.Index < sender.Version.CurrentBlockIndex)
            {
                _logger.LogInformation($"The peer has {sender.Version.CurrentBlockIndex + 1} blocks but the current number of block headers is {_blockchain.LastBlockHeader.Index + 1}.");
                await sender.Send(new GetBlockHeadersMessage(_blockchain.LastBlockHeader.Hash));
            }
        }

        #region Find a better place for block sync

        private static async Task SynchronizeBlocks(IReadOnlyCollection<UInt256> blockHashes, IPeer sender)
        {
            var batchesCount = blockHashes.Count / MaxBlocksCountToSync + (blockHashes.Count % MaxBlocksCountToSync != 0 ? 1 : 0);

            for (var i = 0; i < batchesCount; i++)
            {
                var blockHashesInBatch = blockHashes
                    .Skip(i * MaxBlocksCountToSync)
                    .Take(MaxBlocksCountToSync);

                await sender.Send(new GetDataMessage(InventoryType.Block, blockHashesInBatch));
            }
        }

        #endregion
    }
}