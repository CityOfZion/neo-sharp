using System.Threading.Tasks;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class FilterClearMessageHandler : MessageHandler<FilterClearMessage>
    {
        /// <inheritdoc />
        public override async Task Handle(FilterClearMessage message, IPeer sender)
        {
            sender.BloomFilter = null;

            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is FilterClearMessage;
        }
    }
}