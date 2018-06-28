using System;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class InventoryMessageHandler : BroadcastMessageHandler<InventoryMessage>
    {
        #region Variables

        private readonly ILogger<InventoryMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcast">Broadcast</param>
        /// <param name="logger">Logger</param>
        public InventoryMessageHandler(IBroadcast broadcast, ILogger<InventoryMessageHandler> logger) : base(broadcast)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handle Inventory message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">sender Peer</param>
        /// <returns>Task</returns>
        public override async Task Handle(InventoryMessage message, IPeer sender)
        {
            // TODO: do logic

            // Send broadcast

            switch (message.Payload.Type)
            {
                case InventoryType.Block:
                    {
                        foreach(var hash in message.Payload.Hashes)
                        {

                        }

                        break;
                    }
                case InventoryType.Consensus:
                    {
                        foreach (var hash in message.Payload.Hashes)
                        {

                        }

                        break;
                    }
                case InventoryType.Tx:
                    {
                        foreach (var hash in message.Payload.Hashes)
                        {

                        }

                        break;
                    }
            }

            await base.Handle(message, sender);
        }
    }
}