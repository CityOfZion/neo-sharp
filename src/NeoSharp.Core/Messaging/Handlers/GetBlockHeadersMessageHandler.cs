using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;

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
            var blockHash = message.Payload.HashStart
                .Select(p => _blockchain.GetBlockHeader(p))
                .Where(p => p != null)
                .OrderBy(p => p.Index)
                .Select(p => p.Hash)
                .FirstOrDefault();

            if (blockHash == null || blockHash == message.Payload.HashStop) return;

            var blockHeaders = new List<BlockHeader>();

            do
            {
                blockHash = _blockchain.GetNextBlockHash(blockHash);

                if (blockHash == null || blockHash == message.Payload.HashStop) break;

                blockHeaders.Add(_blockchain.GetBlockHeader(blockHash));
            } while (blockHash != message.Payload.HashStop && blockHeaders.Count < MaxBlockHeadersCountToReturn);

            if (blockHeaders.Count == 0) return;

            await sender.Send(new BlockHeadersMessage(blockHeaders));
        }
    }
}