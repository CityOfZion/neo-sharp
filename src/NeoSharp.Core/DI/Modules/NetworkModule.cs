using System.Linq;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Network;
using NeoSharp.Core.Network.Protocols;
using NeoSharp.Core.Network.Rpc;
using NeoSharp.Core.Network.Security;
using NeoSharp.Core.Network.Tcp;

namespace NeoSharp.Core.DI.Modules
{
    public class NetworkModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IServerContext, ServerContext>();
            containerBuilder.RegisterSingleton<IBlockchainContext, BlockchainContext>();
            containerBuilder.RegisterSingleton<IPeerMessageListener, PeerMessageListener>();

            containerBuilder.RegisterSingleton<NetworkConfig>();
            containerBuilder.RegisterSingleton<RpcConfig>();
            containerBuilder.RegisterSingleton<ProtocolSelector>();
            containerBuilder.RegisterSingleton<INetworkAclLoader, NetworkAclLoader>();
            containerBuilder.RegisterSingleton<INetworkManager, NetworkManager>();
            containerBuilder.RegisterSingleton<IBroadcaster, Server>();
            containerBuilder.RegisterSingleton<IServer, Server>();
            containerBuilder.RegisterSingleton<IRpcServer, RpcServer>();
            containerBuilder.RegisterSingleton<IPeerFactory, PeerFactory>();
            containerBuilder.RegisterSingleton<IPeerListener, TcpPeerListener>();
            containerBuilder.RegisterSingleton<ITcpPeerFactory, TcpPeerFactory>();

            var exportedTypes = typeof(IMessageHandler).Assembly
                .GetExportedTypes()
                .Where(x => x.IsClass && !x.IsAbstract);

            var messageHandlers = exportedTypes.Where(t => t.IsAssignableToGenericType(typeof(MessageHandler<>)));

            containerBuilder.RegisterCollection(typeof(IMessageHandler), messageHandlers);
            containerBuilder.RegisterSingleton<IMessageHandlerProxy, MessageHandlerProxy>();

            var serverProcesses = exportedTypes.Where(t => t.GetInterfaces().Contains(typeof(IServerProcess)));
            containerBuilder.RegisterCollection(typeof(IServerProcess), serverProcesses);
        }
    }
}