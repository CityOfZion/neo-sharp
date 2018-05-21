using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class GetBlockHashesMessageHandler : IMessageHandler<GetBlockHashesMessage>
    {
        private const int MaxBlockHeadersCountToReturn = 2000;
        private readonly IBlockchain _blockchain;

        public GetBlockHashesMessageHandler(IBlockchain blockchain)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
        }

        public async Task Handle(GetBlockHashesMessage message, IPeer sender)
        {
            var blockHash = message.Payload.HashStart
                .Select(p => _blockchain.GetBlockHeader(p))
                .Where(p => p != null)
                .OrderBy(p => p.Index)
                .Select(p => p.Hash)
                .FirstOrDefault();

            if (blockHash == null || blockHash == message.Payload.HashStop) return;

            var blockHashes = new List<UInt256>();

            do
            {
                blockHash = _blockchain.GetNextBlockHash(blockHash);

                if (blockHash == null || blockHash == message.Payload.HashStop) break;

                blockHashes.Add(blockHash);
            } while (blockHash != message.Payload.HashStop && blockHashes.Count < MaxBlockHeadersCountToReturn);

            if (blockHashes.Count == 0) return;

            await sender.Send(new InventoryMessage(InventoryType.Block, blockHashes));
        }
    }
}