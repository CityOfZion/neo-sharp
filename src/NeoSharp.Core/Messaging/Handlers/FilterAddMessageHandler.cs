using System.Threading.Tasks;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class FilterAddMessageHandler : MessageHandler<FilterAddMessage>
    {
        /// <inheritdoc />
        public override async Task Handle(FilterAddMessage message, IPeer sender)
        {
            if (sender.BloomFilter != null)
            {
                sender.BloomFilter.Add(message.Payload.Data);
            }

            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is FilterAddMessage;
        }
    }
}