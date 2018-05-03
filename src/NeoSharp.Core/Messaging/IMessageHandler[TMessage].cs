using System.Threading.Tasks;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging
{
    public interface IMessageHandler<in TMessage> where TMessage : Message
    {
        Task Handle(TMessage message, IPeer sender);
    }
}