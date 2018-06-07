using System;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
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
            sender.Version = message.Payload;

            if (_server.Version.Nonce == sender.Version.Nonce)
            {
                throw new InvalidOperationException($"The handshake is failed due to \"{nameof(_server.Version.Nonce)}\" value equality.");
            }

            if (sender.ChangeProtocol(message.Payload))
            {
                _logger?.LogWarning("Changed protocol.");
            }

            await sender.Send<VerAckMessage>();
        }
    }
}