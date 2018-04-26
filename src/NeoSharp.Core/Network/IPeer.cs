using System.Threading.Tasks;
using NeoSharp.Core.Network.Messages;

namespace NeoSharp.Core.Network
{
    public interface IPeer
    {
        Task Send<TMessage>() where TMessage : Message, new();

        Task Send<TMessage>(TMessage message)
            where TMessage : Message;

        Task<TMessage> Receive<TMessage>()
            where TMessage : Message, new();

        void Disconnect();
    }
}