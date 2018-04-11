using NeoSharp.Core.Network;

namespace NeoSharp.Application.DI
{
    public class NetworkModuleRegister 
    {
        public void Register(ISimpleInjectorWrapper container)
        {
            container.RegisterSingleton<NetworkConfig>();
            container.RegisterSingleton<INetworkManager, NetworkManager>();
            container.RegisterSingleton<IServer, Server>();

            container.RegisterTransientInstance<IPeer, Peer>();
        }
    }

}
