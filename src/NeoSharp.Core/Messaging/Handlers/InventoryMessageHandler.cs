using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;
using System;
using System.Threading.Tasks;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class InventoryMessageHandler : IMessageHandler<InventoryMessage>
    {
        private readonly ILogger<InventoryMessageHandler> _logger;

        public InventoryMessageHandler(ILogger<InventoryMessageHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(InventoryMessage message, IPeer sender)
        {
            // TODO: how to inject server here? we need to send a broadcast and have a circle dependency

            //await _server.SendBroadcast(message, (peer) => peer != sender);
            await Task.CompletedTask;
        }
    }
}