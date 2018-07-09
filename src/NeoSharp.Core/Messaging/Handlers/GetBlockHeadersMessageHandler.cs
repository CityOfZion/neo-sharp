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
            var blockHeadersFromPayload = new List<BlockHeader>();

            foreach (var hash in message.Payload.HashStart)
            {
                var blockHeader = await this._blockchain.GetBlockHeader(hash);

                if (blockHeader != null)
                {
                    blockHeadersFromPayload.Add(blockHeader);
                }
            }

            var blockHash = blockHeadersFromPayload
                .OrderBy(x => x.Index)
                .Select(x => x.Hash)
                .FirstOrDefault();

            if (blockHash == null || blockHash == message.Payload.HashStop) return;

            var blockHeaders = new List<BlockHeader>();

            do
            {
                blockHash = await this._blockchain.GetNextBlockHash(blockHash);

                if (blockHash == null || blockHash == message.Payload.HashStop) break;

                blockHeaders.Add(await this._blockchain.GetBlockHeader(blockHash));
            } while (blockHash != message.Payload.HashStop && blockHeaders.Count < MaxBlockHeadersCountToReturn);

            if (blockHeaders.Count == 0) return;

            await sender.Send(new BlockHeadersMessage(blockHeaders));
        }
    }
}