using System;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class VerAckMessageHandler : MessageHandler<VerAckMessage>
    {
        #region Private Fields 

        private readonly IBlockchainContext _blockchainContext;
        private readonly ILogger<VerAckMessageHandler> _logger;

        #endregion

        #region Constructor 

        public VerAckMessageHandler(
            IBlockchainContext blockchainContext,
            ILogger<VerAckMessageHandler> logger)
        {
            _blockchainContext = blockchainContext ?? throw new ArgumentNullException(nameof(blockchainContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region MessageHandler override methods

        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is VerAckMessage;
        }

        /// <inheritdoc />
        public override async Task Handle(VerAckMessage message, IPeer sender)
        {
            sender.IsReady = true;
            _blockchainContext.SetPeerCurrentBlockIndex(sender.Version.CurrentBlockIndex);

            if (_blockchainContext.NeedPeerSync)
            {
                _logger.LogInformation($"The peer has {sender.Version.CurrentBlockIndex + 1} blocks but the current number of block headers is {_blockchainContext.LastBlockHeader.Index + 1}.");

                await sender.Send(new GetBlockHeadersMessage(_blockchainContext.LastBlockHeader.Hash));
            }
        }

        #endregion
    }
}