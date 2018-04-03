using NeoSharp.Core.Network;
using SimpleInjector;

namespace NeoSharp.Application.DI
{
    public static class NetworkPackage
    {
        public static void RegisterServices(Container container)
        {
            container.Register<NetworkConfig>(Lifestyle.Singleton);
            container.Register<INetworkManager, NetworkManager>(Lifestyle.Singleton);
            container.Register<IServer, Server>(Lifestyle.Singleton);
            container.RegisterInstanceCreator<IPeer, Peer>(Lifestyle.Transient);
        }     
    }

}
