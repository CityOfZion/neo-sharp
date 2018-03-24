using System;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace NeoSharp.Network.DI
{
    public static class NetworkPackage
    {
        public static void RegisterServices(Container container)
        {                
            container.Register<INetworkManager, NetworkManager>(Lifestyle.Singleton);
            container.Register<IServer, Server>(Lifestyle.Singleton);
            container.Register<IPeer, Peer>(Lifestyle.Transient);
            container.Register<IPeerFactory, PeerFactory>(Lifestyle.Singleton);
        }     
    }

}
