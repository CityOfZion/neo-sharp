using NeoSharp.Core.DI;
using NeoSharp.Core.Network;
using NeoSharp.Core.Network.Tcp;

namespace NeoSharp.Application.DI
{
    public class NetworkModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<NetworkConfig>();
            containerBuilder.RegisterSingleton<INetworkManager, NetworkManager>();
            containerBuilder.RegisterSingleton<IServer, Server>();
            containerBuilder.RegisterSingleton<IPeerFactory, PeerFactory>();
            containerBuilder.RegisterSingleton<IPeerListener, TcpPeerListener>();
            containerBuilder.RegisterSingleton<ITcpPeerFactory, TcpPeerFactory>();
        }
    }
}
