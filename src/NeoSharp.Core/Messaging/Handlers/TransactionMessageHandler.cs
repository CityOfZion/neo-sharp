using System;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class TransactionMessageHandler : IMessageHandler<TransactionMessage>
    {
        #region Variables

        private readonly IBroadcaster _broadcaster;
        private readonly ILogger<TransactionMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcaster">Broadcaster</param>
        /// <param name="logger">Logger</param>
        public TransactionMessageHandler(IBroadcaster broadcaster, ILogger<TransactionMessageHandler> logger)
        {
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task Handle(TransactionMessage message, IPeer sender)
        {
            foreach (var tx in message.Payload.Transactions)
            {

            }

            return Task.CompletedTask;
        }
    }
}