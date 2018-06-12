using System;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class TxMessageHandler : InventoryMessageHandler<TxMessage>
    {
        #region Variables

        private readonly ILogger<TxMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcast">Broadcast</param>
        /// <param name="logger">Logger</param>
        public TxMessageHandler(IBroadcast broadcast, ILogger<TxMessageHandler> logger) : base(broadcast)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}