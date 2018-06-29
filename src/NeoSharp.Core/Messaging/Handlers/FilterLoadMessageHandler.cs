using System;
using System.Threading.Tasks;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class FilterLoadMessageHandler : IMessageHandler<FilterLoadMessage>
    {
        #region Variables

        private readonly ICrypto _crypto;
        private readonly ILogger<FilterLoadMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="crypto">Crypto</param>
        /// <param name="logger">Logger</param>
        public FilterLoadMessageHandler(ICrypto crypto, ILogger<FilterLoadMessageHandler> logger)
        {
            _crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handle FilterLoad message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">Sender</param>
        /// <returns>Task</returns>
        public async Task Handle(FilterLoadMessage message, IPeer sender)
        {
            var payload = message.Payload;

            sender.BloomFilter = new BloomFilter(
                _crypto,
                payload.Filter.Length * 2,
                payload.K,
                payload.Tweak,
                payload.Filter);

            await Task.CompletedTask;
        }
    }
}