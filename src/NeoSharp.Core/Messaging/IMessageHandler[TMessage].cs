using System.Threading.Tasks;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging
{
    public abstract class MessageHandler<TMessage> : IMessageHandler
        where TMessage : Message
    {
        public abstract bool CanHandle(Message message);

        public abstract Task Handle(TMessage message, IPeer sender);
    }

    public interface IMessageHandler
    {
        bool CanHandle(Message message);
    }
}