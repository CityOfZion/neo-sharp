using System;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class FilterClearMessageHandler : IMessageHandler<FilterClearMessage>
    {
        #region Variables

        private readonly ILogger<FilterClearMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public FilterClearMessageHandler(ILogger<FilterClearMessageHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handle GetMemPool message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">Sender</param>
        /// <returns>Task</returns>
        public async Task Handle(FilterClearMessage message, IPeer sender)
        {
            sender.BloomFilter = null;

            await Task.CompletedTask;
        }
    }
}