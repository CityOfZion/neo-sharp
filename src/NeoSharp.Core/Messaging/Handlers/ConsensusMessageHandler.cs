using System;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class ConsensusMessageHandler : IMessageHandler<ConsensusMessage>
    {
        #region Variables

        private readonly IBroadcaster _broadcaster;
        private readonly ILogger<ConsensusMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcaster">Broadcaster</param>
        /// <param name="logger">Logger</param>
        public ConsensusMessageHandler(IBroadcaster broadcaster, ILogger<ConsensusMessageHandler> logger)
        {
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(ConsensusMessage message, IPeer sender)
        {
            await _broadcaster.Broadcast(message, sender);
        }
    }
}