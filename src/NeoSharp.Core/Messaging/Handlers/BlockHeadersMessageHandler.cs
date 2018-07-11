using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class BlockHeadersMessageHandler : IMessageHandler<BlockHeadersMessage>
    {
        private readonly IBlockchain _blockchain;
        private readonly IBinarySerializer _serializier;
        private readonly ICrypto _crypto;
        private readonly ILogger<BlockHeadersMessageHandler> _logger;

        public BlockHeadersMessageHandler(IBlockchain blockchain, ILogger<BlockHeadersMessageHandler> logger, IBinarySerializer serializier, ICrypto crypto)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serializier = serializier ?? throw new ArgumentNullException(nameof(serializier));
            _crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
        }

        public async Task Handle(BlockHeadersMessage message, IPeer sender)
        {
            var indexH = _blockchain.LastBlockHeader.Index;
            var notFoundHeaders = new List<BlockHeaderBase>();

            foreach (var header in message.Payload.Headers)
            {
                var needHeader = indexH < header.Header.Index;

                if (!needHeader) continue;

                // Compute only necessary hashes

                header.Header.UpdateHash(_serializier, _crypto);
                notFoundHeaders.Add(header.Header);
            }

            // Add only if is not in the chain

            await _blockchain.AddBlockHeaders(notFoundHeaders);

            // TODO: Change Block-logic sync task

            await RequestLeftBlocks(sender);
        }

        #region Move this to one task

        private async Task RequestLeftBlocks(IPeer sender)
        {
            // Max 500

            var split = 500U;

            // Request blocks

            for (uint ix = _blockchain.CurrentBlock.Index + 1, max = _blockchain.LastBlockHeader.Index; ix < max; ix += split)
            {
                await sender.Send(new GetDataMessage(InventoryType.Block, GetHashes(ix, ix + split)));
            }
        }

        private IEnumerable<UInt256> GetHashes(uint from, uint to)
        {
            for (; from < to; from++)
            {
                var header = _blockchain.GetBlockHeader(from);

                header.Wait();

                if (header.Result == null) yield break;

                yield return header.Result.Hash;
            }
        }

        #endregion
    }
}