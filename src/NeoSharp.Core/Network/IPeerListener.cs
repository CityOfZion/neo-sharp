using System;

namespace NeoSharp.Core.Network
{
    public interface IPeerListener
    {
        event EventHandler<IPeer> PeerConnected;
        void Start();
        void Stop();
    }
}