using NeoSharp.Core.DI;
using NeoSharp.Core.Network;

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
        }
    }
}
