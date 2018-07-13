using System;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class BlockMessageHandler : IMessageHandler<BlockMessage>
    {
        #region Variables

        private readonly ILogger<BlockMessageHandler> _logger;
        private readonly IBlockchain _blockchain;
        private readonly IBroadcaster _broadcaster;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockchain">Blockchain</param>
        /// <param name="broadcaster">Broadcaster</param>
        /// <param name="logger">Logger</param>
        public BlockMessageHandler(IBlockchain blockchain, IBroadcaster broadcaster, ILogger<BlockMessageHandler> logger)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(BlockMessage message, IPeer sender)
        {
            var block = message.Payload;
            if (block == null) return;

            if (block.Hash == null)
            {
                block.UpdateHash(BinarySerializer.Default, ICrypto.Default);
            }

            var blockExists = await _blockchain.ContainsBlock(block.Hash);
            if (blockExists)
            {
                _logger.LogInformation($"The block \"{block.Hash.ToString(true)}\" exists already on the blockchain.");
                return;
            }

            var blockAdded = await _blockchain.AddBlock(block);
            if (!blockAdded)
            {
                _logger.LogWarning($"The block \"{block.Hash.ToString(true)}\" was not added to the blockchain.");
                return;
            }

            _broadcaster.Broadcast(message, sender);
        }
    }
}
