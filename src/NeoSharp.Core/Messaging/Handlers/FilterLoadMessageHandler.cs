using System.Threading.Tasks;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class FilterLoadMessageHandler : MessageHandler<FilterLoadMessage>
    {
        /// <inheritdoc />
        public override async Task Handle(FilterLoadMessage message, IPeer sender)
        {
            var payload = message.Payload;

            sender.BloomFilter = new BloomFilter(
                payload.Filter.Length * 2,
                payload.K,
                payload.Tweak,
                payload.Filter);

            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is FilterLoadMessage;
        }
    }
}