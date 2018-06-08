using System;
using Moq;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Test.ExtensionMethods
{
    public static class ServerContectMockExtensionMethods
    {
        public static Mock<IServerContext> SetupDefaultServerContext(this Mock<IServerContext> serverContextMock)
        {
            var payload = new VersionPayload
            {
                Version = 1,
                Services = 1,
                Nonce = (uint)new Random(Environment.TickCount).Next(int.MaxValue),
            };

            serverContextMock
                .SetupGet(x => x.Version)
                .Returns(payload);

            return serverContextMock;
        }
    }
}
