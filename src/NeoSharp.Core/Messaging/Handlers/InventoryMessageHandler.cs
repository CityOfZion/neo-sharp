using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class InventoryMessageHandler : MessageHandler<InventoryMessage>
    {
        #region Private Fields
        private readonly ILogger<InventoryMessageHandler> _logger;
        #endregion

        #region Constructor 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public InventoryMessageHandler(ILogger<InventoryMessageHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region MessageHandler override methods 
        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is InventoryMessage;
        }

        /// <inheritdoc />
        public override Task Handle(InventoryMessage message, IPeer sender)
        {
            var inventoryType = message.Payload.Type;
            if (Enum.IsDefined(typeof(InventoryType), inventoryType) == false)
            {
                _logger.LogError($"The payload of {nameof(InventoryMessage)} contains unknown {nameof(InventoryType)} \"{inventoryType}\".");

                return Task.CompletedTask;
            }

            var hashes = message.Payload.Hashes
                .Distinct()
                .ToArray();

            // TODO #375: exclude known hashes

            if (!hashes.Any())
            {
                _logger.LogWarning($"The payload of {nameof(InventoryMessage)} contains no hashes.");

                return Task.CompletedTask;
            }


            return Task.CompletedTask;
            //return sender.Send(new GetDataMessage(inventoryType, hashes));
        }
        #endregion
    }
}