using System;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class TransactionMessageHandler : InventoryMessageHandler<TransactionMessage>
    {
        #region Variables

        private readonly ILogger<TransactionMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcast">Broadcast</param>
        /// <param name="logger">Logger</param>
        public TransactionMessageHandler(IBroadcast broadcast, ILogger<TransactionMessageHandler> logger) : base(broadcast)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}