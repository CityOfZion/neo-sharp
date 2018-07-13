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
    public class GetBlockHeadersMessageHandler : IMessageHandler<GetBlockHeadersMessage>
    {
        private const int MaxBlockHeadersCountToReturn = 2000;
        private readonly IBlockchain _blockchain;

        public GetBlockHeadersMessageHandler(IBlockchain blockchain)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
        }

        public async Task Handle(GetBlockHeadersMessage message, IPeer sender)
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

            var blockHeaders = new List<BlockHeader>();

            do
            {
                blockHash = await _blockchain.GetNextBlockHash(blockHash);

                if (blockHash == null || blockHash == hashStop) break;

                blockHeaders.Add(await _blockchain.GetBlockHeader(blockHash));
            } while (blockHeaders.Count < MaxBlockHeadersCountToReturn);

            if (blockHeaders.Count == 0) return;

            await sender.Send(new BlockHeadersMessage(blockHeaders));
        }

        private Task<BlockHeader> GetBlockHeader(UInt256 hash) => _blockchain.GetBlockHeader(hash);
    }
}