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
    public class GetBlockHeadersMessageHandler : MessageHandler<GetBlockHeadersMessage>
    {
        #region Private Fields 
        private const int MaxBlockHeadersCountToReturn = 2000;
        private readonly IBlockRepository _blockRepository;

        private Task<BlockHeader> GetBlockHeader(UInt256 hash) => this._blockRepository.GetBlockHeader(hash);
        #endregion

        #region Constructor 
        public GetBlockHeadersMessageHandler(IBlockRepository blockModel)
        {
            _blockRepository = blockModel ?? throw new ArgumentNullException(nameof(blockModel));
        }
        #endregion

        #region MessageHandler override methods
        /// <inheritdoc />
        public override async Task Handle(GetBlockHeadersMessage message, IPeer sender)
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
                blockHash = await this._blockRepository.GetNextBlockHash(blockHash);

                if (blockHash == null || blockHash == hashStop) break;

                blockHeaders.Add(await this._blockRepository.GetBlockHeader(blockHash));
            } while (blockHeaders.Count < MaxBlockHeadersCountToReturn);

            if (blockHeaders.Count == 0) return;

            await sender.Send(new BlockHeadersMessage(blockHeaders));
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is GetBlockHeadersMessage;
        }
        #endregion
    }
}