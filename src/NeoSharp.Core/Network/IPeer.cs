using System.Threading.Tasks;
using NeoSharp.Core.Messaging;

namespace NeoSharp.Core.Network
{
    public interface IPeer
    {
        bool IsConnected { get; }

        bool IsReady { get; set; }

        void DowngradeProtocol(uint version);

        Task Send(Message message);

        Task Send<TMessage>() where TMessage : Message, new();

        Task<Message> Receive();

        Task<TMessage> Receive<TMessage>() where TMessage : Message, new();

        void Disconnect();
    }
}