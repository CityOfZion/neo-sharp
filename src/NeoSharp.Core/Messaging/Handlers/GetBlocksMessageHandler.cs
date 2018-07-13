using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class GetBlocksMessageHandler : IMessageHandler<GetBlocksMessage>
    {
        private const int MaxBlocksCountToReturn = 500;
        private readonly IBlockchain _blockchain;

        public GetBlocksMessageHandler(IBlockchain blockchain)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
        }

        public async Task Handle(GetBlocksMessage message, IPeer sender)
        {
            var hashStart = (message.Payload.HashStart ?? new UInt256[0])
                .Where(h => h != null)
                .Distinct()
                .ToArray();

            if (hashStart.Length == 0) return;

            var hashStop = message.Payload.HashStop;

            var blockHash = (await Task.WhenAll(hashStart.Select(GetBlockHeader)))
                .Where(bh => bh != null)
                .OrderBy(bh => bh.Index)
                .Select(bh => bh.Hash)
                .FirstOrDefault();

            if (blockHash == null || blockHash == hashStop) return;

            var blockHashes = new List<UInt256>();

            do
            {
                blockHash = await _blockchain.GetNextBlockHash(blockHash);

                if (blockHash == null || blockHash == hashStop) break;

                blockHashes.Add(blockHash);
            } while (blockHashes.Count < MaxBlocksCountToReturn);

            if (blockHashes.Count == 0) return;

            await sender.Send(new InventoryMessage(InventoryType.Block, blockHashes));
        }

        private Task<BlockHeader> GetBlockHeader(UInt256 hash) => _blockchain.GetBlockHeader(hash);
    }
}