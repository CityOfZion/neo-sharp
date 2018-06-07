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

            if (_server.Nonce == sender.Version.Nonce)
            {
                throw new InvalidOperationException($"The handshake is failed due to \"{nameof(_server.Nonce)}\" value equality.");
            }

            if (_server.ProtocolVersion > sender.Version.Version)
            {
                _logger.LogWarning("Downgraded to a lower protocol version.");
                sender.DowngradeProtocol(sender.Version.Version);
            }

            await sender.Send<VerAckMessage>();
        }
    }
}