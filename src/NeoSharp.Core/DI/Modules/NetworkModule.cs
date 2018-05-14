using System.Linq;
using Microsoft.Extensions.Logging;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Handlers;
using NeoSharp.Core.Network;
using NeoSharp.Core.Network.Protocols;
using NeoSharp.Core.Network.Tcp;

namespace NeoSharp.Core.DI.Modules
{
    public class NetworkModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<NetworkConfig>();
            containerBuilder.RegisterSingleton<ProtocolSelector>();
            containerBuilder.RegisterSingleton<INetworkManager, NetworkManager>();
            containerBuilder.RegisterSingleton<IServer, Server>();
            containerBuilder.RegisterSingleton<IPeerFactory, PeerFactory>();
            containerBuilder.RegisterSingleton<IPeerListener, TcpPeerListener>();
            containerBuilder.RegisterSingleton<ITcpPeerFactory, TcpPeerFactory>();

            var messageHandlerTypes = typeof(VersionMessageHandler).Assembly
                .GetExportedTypes()
                .Where(t => t.IsClass && t.IsAssignableToGenericType(typeof(IMessageHandler<>)) &&
                            t != typeof(MessageHandlerProxy))
                .ToArray();

            containerBuilder.Register(typeof(IMessageHandler<>), messageHandlerTypes);
            containerBuilder.RegisterInstanceCreator<IMessageHandler<Message>>(c =>
                new MessageHandlerProxy(c, messageHandlerTypes, c.Resolve<ILogger<MessageHandlerProxy>>()));
        }
    }
}
