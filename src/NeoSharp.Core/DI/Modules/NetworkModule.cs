using System.Linq;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Handlers;
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

            var messageHandlerTypes = typeof(VersionMessageHandler).Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableToGenericType(typeof(IMessageHandler<>)) &&
                            t != typeof(MessageHandlerProxy))
                .ToArray();

            containerBuilder.RegisterCollection(typeof(IMessageHandler<>), messageHandlerTypes);
            containerBuilder.RegisterInstanceCreator<IMessageHandler<Message>>(c =>
                new MessageHandlerProxy(c, messageHandlerTypes, c.Resolve<ILogger<MessageHandlerProxy>>()));
        }
    }
}