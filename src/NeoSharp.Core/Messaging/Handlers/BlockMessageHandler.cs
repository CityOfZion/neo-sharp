using System;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class BlockMessageHandler : InventoryMessageHandler<BlockMessage>
    {
        #region Variables

        private readonly ILogger<BlockMessageHandler> _logger;
        private readonly IBlockchain _blockchain;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockchain">Blockchain</param>
        /// <param name="broadcast">Broadcast</param>
        /// <param name="logger">Logger</param>
        public BlockMessageHandler(IBlockchain blockchain, IBroadcast broadcast, ILogger<BlockMessageHandler> logger) : base(broadcast)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
        }

        public override async Task Handle(BlockMessage message, IPeer sender)
        {
            if (message.Payload != null)
            {
                // TODO: Pools, Queues, orders, etc, etc

                await _blockchain.AddBlock(message.Payload);
            }

            // TODO: Shall I relay it?

            await base.Handle(message, sender);
        }
    }
}