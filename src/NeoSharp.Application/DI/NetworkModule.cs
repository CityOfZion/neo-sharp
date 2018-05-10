using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using NeoSharp.Core.DI;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Handlers;
using NeoSharp.Core.Network;
using NeoSharp.Core.Network.Protocols;
using NeoSharp.Core.Network.Tcp;

namespace NeoSharp.Application.DI
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
                .Where(t => t.IsClass &&
                            IsAssignableToGenericType(t, typeof(IMessageHandler<>)) &&
                            t != typeof(MessageHandlerProxy))
                .ToArray();

            containerBuilder.Register(typeof(IMessageHandler<>), messageHandlerTypes);
            containerBuilder.RegisterInstanceCreator<IMessageHandler<Message>>(c =>
                new MessageHandlerProxy(c, messageHandlerTypes, c.Resolve<ILogger<MessageHandlerProxy>>()));
        }

        private static bool IsAssignableToGenericType(Type givenType, Type openGenericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == openGenericType))
            {
                return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == openGenericType)
                return true;

            var baseType = givenType.BaseType;

            return baseType != null && IsAssignableToGenericType(baseType, openGenericType);
        }
    }
}
