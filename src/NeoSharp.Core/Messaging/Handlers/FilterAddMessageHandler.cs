using System;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class FilterAddMessageHandler : IMessageHandler<FilterAddMessage>
    {
        #region Variables

        private readonly ILogger<FilterAddMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        public FilterAddMessageHandler(ILogger<FilterAddMessageHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handle FilterAdd message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">Sender</param>
        /// <returns>Task</returns>
        public async Task Handle(FilterAddMessage message, IPeer sender)
        {
            if (sender.BloomFilter != null)
            {
                sender.BloomFilter.Add(message.Payload.Data);
            }

            await Task.CompletedTask;
        }
    }
}