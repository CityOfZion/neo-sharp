namespace NeoSharp.Core.Network
{
    public interface IPeerMessageListener
    {
        void StartListen(IPeer peer);

        void StopListenAllPeers();
    }
}
