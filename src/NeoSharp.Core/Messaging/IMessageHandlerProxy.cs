using System.Threading.Tasks;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging
{
    public interface IMessageHandlerProxy
    {
        Task Handle(Message message, IPeer sender);
    }
}