using System;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class BlockMessageHandler : MessageHandler<BlockMessage>
    {
        #region Private Fields 
        private readonly IBlockProcessor _blockProcessor;
        private readonly IBlockSigner _blockSigner;
        private readonly IBlockVerifier _blockVerifier;
        private readonly IBroadcaster _broadcaster;
        private readonly ILogger<BlockMessageHandler> _logger;
        #endregion

        #region Constructor 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockProcessor">Block Pool</param>
        /// <param name="blockSigner">Block operations manager.</param>
        /// <param name="broadcaster">Broadcaster</param>
        /// <param name="logger">Logger</param>
        public BlockMessageHandler(
            IBlockProcessor blockProcessor,
            IBlockSigner blockSigner,
            IBlockVerifier blockVerifier,
            IBroadcaster broadcaster,
            ILogger<BlockMessageHandler> logger)
        {
            this._blockProcessor = blockProcessor ?? throw new ArgumentNullException(nameof(blockProcessor));
            this._blockSigner = blockSigner;
            this._blockVerifier = blockVerifier;
            this._broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            this._logger = logger;
        }
        #endregion

        #region MessageHandler override methods 
        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is BlockMessage;
        }

        /// <inheritdoc />
        public override async Task Handle(BlockMessage message, IPeer sender)
        {
            var block = message.Payload;
            
            if (block.Hash == null)
            {
                this._blockSigner.Sign(block);
            }

            if (_blockVerifier.Verify(block))
            {
                await _blockProcessor.AddBlock(block);
            }

            this._logger.LogInformation($"Adding block {block.Hash} to the BlockPool with Index {block.Index}.");
            await this._blockProcessor.AddBlock(block);

            this._logger.LogInformation($"Broadcasting block {block.Hash} with Index {block.Index}.");
            this._broadcaster.Broadcast(message, sender);
        }
        #endregion
    }
}
