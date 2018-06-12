using System;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class ConsensusMessageHandler : InventoryMessageHandler<ConsensusMessage>
    {
        #region Variables

        private readonly ILogger<ConsensusMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcast">Broadcast</param>
        /// <param name="logger">Logger</param>
        public ConsensusMessageHandler(IBroadcast broadcast, ILogger<ConsensusMessageHandler> logger) : base(broadcast)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}