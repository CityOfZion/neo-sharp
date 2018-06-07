using System;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class VersionMessageHandler : IMessageHandler<VersionMessage>
    {
        #region Variables

        private readonly IServer _server;
        private readonly ILogger<VersionMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="server">Server</param>
        /// <param name="logger">Logger</param>
        public VersionMessageHandler(IServer server, ILogger<VersionMessageHandler> logger)
        {
            _server = server;
            _logger = logger;
        }

        /// <summary>
        /// Handle message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">Sender</param>
        /// <returns>Task</returns>
        public async Task Handle(VersionMessage message, IPeer sender)
        {
            sender.Version = message.Payload;

            if (_server.Version.Nonce == sender.Version.Nonce)
            {
                throw new InvalidOperationException($"The handshake is failed due to \"{nameof(_server.Version.Nonce)}\" value equality.");
            }

            // Send Ack

            await sender.Send<VerAckMessage>();

            // Change protocol?

            if (sender.ChangeProtocol(message.Payload))
            {
                _logger?.LogWarning("Changed protocol.");
            }
        }
    }
}