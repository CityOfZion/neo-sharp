using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class BlockHeadersMessageHandler : MessageHandler<BlockHeadersMessage>
    {
        #region Private Fields 
        private readonly IBlockchainContext _blockchainContext;
        private readonly IBlockPersister _blockPersister;
        private readonly ILogger<BlockHeadersMessageHandler> _logger;
        #endregion

        #region Constructor 

        public BlockHeadersMessageHandler
            (
            IBlockchainContext blockchainContext,
            IBlockPersister blockPersister,
            ILogger<BlockHeadersMessageHandler> logger
            )
        {
            _blockchainContext = blockchainContext ?? throw new ArgumentNullException(nameof(blockchainContext));
            _blockPersister = blockPersister ?? throw new ArgumentNullException(nameof(blockPersister));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region MessageHandler override Methods 
        /// <inheritdoc />
        public override async Task Handle(BlockHeadersMessage message, IPeer sender)
        {
            message.Payload.Headers.ForEach(a => a.Type = HeaderType.Header);

            await _blockPersister.Persist(message.Payload.Headers ?? new BlockHeader[0]);

            if (_blockchainContext.LastBlockHeader.Index < sender.Version.CurrentBlockIndex)
            {
                _logger.LogInformation(
                    $"The peer has {sender.Version.CurrentBlockIndex + 1} blocks but the current number of block headers is {_blockchainContext.LastBlockHeader.Index + 1}.");
                await sender.Send(new GetBlockHeadersMessage(_blockchainContext.LastBlockHeader.Hash));
            }
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message) => message is BlockHeadersMessage;

        #endregion
    }
}