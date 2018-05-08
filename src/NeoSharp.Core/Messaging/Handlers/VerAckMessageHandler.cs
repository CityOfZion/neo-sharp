using System.Threading.Tasks;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class VerAckMessageHandler : IMessageHandler<VerAckMessage>
    {
        public Task Handle(VerAckMessage message, IPeer sender)
        {
            sender.IsReady = true;

            return Task.CompletedTask;
        }
    }
}