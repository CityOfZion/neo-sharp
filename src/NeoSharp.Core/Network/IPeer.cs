using System;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Network
{
    public interface IPeer
    {
        bool IsConnected { get; }

        BloomFilter BloomFilter { get; set; }

        EndPoint EndPoint { get; }

        VersionPayload Version { get; set; }

        bool IsReady { get; set; }

        DateTime ConnectionDate { get; }

        bool ChangeProtocol(VersionPayload version);

        Task Send(Message message);

        Task Send<TMessage>() where TMessage : Message, new();

        Task<Message> Receive();

        Task<TMessage> Receive<TMessage>() where TMessage : Message, new();

        void Disconnect();

        event EventHandler OnDisconnect;
    }
}