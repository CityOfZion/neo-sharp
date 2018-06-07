using NeoSharp.Core.Messaging.Messages;
using System.Collections.Generic;

namespace NeoSharp.Core.Network
{
    public interface IServer
    {
        void Start();

        void Stop();

        IReadOnlyCollection<IPeer> ConnectedPeers { get; }

        VersionPayload Version { get; }
    }
}