using System;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Processors;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class BlockMessageHandler : IMessageHandler<BlockMessage>
    {
        #region Variables

        private readonly ILogger<BlockMessageHandler> _logger;
        private readonly IBlockProcessor _blockProcessor;
        private readonly IBroadcaster _broadcaster;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockProcessor">Block Processor</param>
        /// <param name="broadcaster">Broadcaster</param>
        /// <param name="logger">Logger</param>
        public BlockMessageHandler(IBlockProcessor blockProcessor, IBroadcaster broadcaster, ILogger<BlockMessageHandler> logger)
        {
            _blockProcessor = blockProcessor ?? throw new ArgumentNullException(nameof(blockProcessor));
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(BlockMessage message, IPeer sender)
        {
            var block = message.Payload;
            if (block == null) return;

            var blockExists = await _blockProcessor.ContainsBlock(block.Hash);
            if (blockExists)
            {
                _logger.LogInformation($"The block \"{block.Hash.ToString(true)}\" exists already on the blockchain.");
                return;
            }

            _blockProcessor.AddBlock(block);
            _broadcaster.Broadcast(message, sender);
        }
    }
}
