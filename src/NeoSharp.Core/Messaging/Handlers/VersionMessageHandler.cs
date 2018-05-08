using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class VersionMessageHandler : IMessageHandler<VersionMessage>
    {
        private readonly IServer _server;
        private readonly ILogger<VersionMessageHandler> _logger;

        public VersionMessageHandler(IServer server, ILogger<VersionMessageHandler> logger)
        {
            _server = server;
            _logger = logger;
        }

        public async Task Handle(VersionMessage message, IPeer sender)
        {
            if (_server.Nonce != message.Payload.Nonce)
            {
                throw new InvalidOperationException("The handshake failed.");
            }

            if (_server.Version > message.Payload.Version)
            {
                _logger.LogWarning("Downgraded to lower protocol version.");
                sender.DowngradeProtocol(message.Payload.Version);
            }

            await sender.Send<VerAckMessage>();
        }
    }
}