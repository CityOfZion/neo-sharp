using NeoSharp.Core.Network.Messages;
using System.Threading.Tasks;

namespace NeoSharp.Core.Network
{
    public interface IPeer
    {
        Task Send<TMessage>(TMessage message) where TMessage : Message, new();

        Task<Message> Receive();
        Task<TMessage> Receive<TMessage>() where TMessage : Message, new();

        void Disconnect();
    }
}