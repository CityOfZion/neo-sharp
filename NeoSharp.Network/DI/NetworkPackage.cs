using SimpleInjector;
using Microsoft.Extensions.Configuration;
using NeoSharp.Modules;

namespace NeoSharp.Network.DI
{
    public static class NetworkPackage
    {
        public static void RegisterServices(Container container)
        {
            NetworkConfig cfg = new NetworkConfig();
            ConfigManager loader = new ConfigManager();
            loader.LoadConfig().GetSection(nameof(NetworkConfig)).Bind(cfg);
            container.RegisterInstance(typeof(NetworkConfig), cfg);

            container.Register<INetworkManager, NetworkManager>(Lifestyle.Singleton);
            container.Register<IServer, Server>(Lifestyle.Singleton);
            container.Register<IPeer, Peer>(Lifestyle.Transient);
            container.Register<IPeerFactory, PeerFactory>(Lifestyle.Singleton);
        }     
    }

}
