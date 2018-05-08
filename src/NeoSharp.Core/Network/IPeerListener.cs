using System;

namespace NeoSharp.Core.Network
{
    public interface IPeerListener
    {
        event EventHandler<IPeer> OnPeerConnected;
        void Start();
        void Stop();
    }
}