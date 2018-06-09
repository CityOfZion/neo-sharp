using System;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class VersionMessageHandler : IMessageHandler<VersionMessage>
    {
        private readonly IServerContext _serverContext;
        private readonly ILogger<VersionMessageHandler> _logger;

        public VersionMessageHandler(IServerContext serverContext, ILogger<VersionMessageHandler> logger)
        {
            _serverContext = serverContext;
            _logger = logger;
        }

        public async Task Handle(VersionMessage message, IPeer sender)
        {
            sender.Version = message.Payload;

            if (_serverContext.Version.Nonce == sender.Version.Nonce)
            {
                throw new InvalidOperationException($"The handshake is failed due to \"{nameof(_serverContext.Version.Nonce)}\" value equality.");
            }

            if (sender.ChangeProtocol(message.Payload))
            {
                _logger?.LogWarning("Changed protocol.");
            }

            await sender.Send<VerAckMessage>();
        }
    }
}