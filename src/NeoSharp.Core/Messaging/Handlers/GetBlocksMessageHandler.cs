using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Types;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class GetBlocksMessageHandler : MessageHandler<GetBlocksMessage>
    {
        #region Private Fields 
        private const int MaxBlocksCountToReturn = 500;
        private readonly IBlockRepository _blockRepository;

        private Task<BlockHeader> GetBlockHeader(UInt256 hash) => this._blockRepository.GetBlockHeader(hash);
        #endregion

        #region Constructor 
        public GetBlocksMessageHandler(IBlockRepository blockModel)
        {
            this._blockRepository = blockModel ?? throw new ArgumentNullException(nameof(blockModel));
        }
        #endregion

        #region MessageHandler override methods
        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is GetBlocksMessage;
        }

        /// <inheritdoc />
        public override async Task Handle(GetBlocksMessage message, IPeer sender)
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
                blockHash = await this._blockRepository.GetNextBlockHash(blockHash);

                if (blockHash == null || blockHash == hashStop) break;

                blockHashes.Add(blockHash);
            } while (blockHashes.Count < MaxBlocksCountToReturn);

            if (blockHashes.Count == 0) return;

            await sender.Send(new InventoryMessage(InventoryType.Block, blockHashes));
        }
        #endregion
    }
}