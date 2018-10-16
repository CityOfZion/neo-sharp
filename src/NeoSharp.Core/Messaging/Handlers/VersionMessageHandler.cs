using System;
using System.Threading.Tasks;
using NeoSharp.Core.Exceptions;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class VersionMessageHandler : MessageHandler<VersionMessage>
    {
        #region Private Fields 
        private readonly ILogger<VersionMessageHandler> _logger;
        private readonly IServerContext _serverContext;
        #endregion

        #region Constructor 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="serverContext">Server</param>
        /// <param name="logger">Logger</param>
        public VersionMessageHandler(IServerContext serverContext, ILogger<VersionMessageHandler> logger)
        {
            _serverContext = serverContext;
            _logger = logger;
        }
        #endregion

        #region MessageHandler override methods
        /// <inheritdoc />
        public override async Task Handle(VersionMessage message, IPeer sender)
        {
            sender.Version = message.Payload;
            if (_serverContext.Version.Nonce == sender.Version.Nonce)
            {
                throw new InvalidMessageException($"The handshake is failed due to \"{nameof(_serverContext.Version.Nonce)}\" value equality.");
            }

            // Change protocol?
            if (sender.ChangeProtocol(message.Payload))
            {
                _logger?.LogWarning("Changed protocol.");
            }

            // Send Ack
            await sender.Send<VerAckMessage>();
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is VersionMessage;
        }
        #endregion
    }
}