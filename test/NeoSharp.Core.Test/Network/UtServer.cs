using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UtServer : TestBase
    {
        [TestMethod]
        public void Can_connect_to_peers_on_start()
        {
            // Arrange 
            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));
            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerFactoryMock.Setup(x => x.ConnectTo(It.IsAny<EndPoint>())).Returns(Task.FromResult(peerMock.Object));
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();

            // Asset
            peerFactoryMock.Verify(x => x.ConnectTo(It.IsAny<EndPoint>()), Times.Once);
        }

        [TestMethod]
        public void Can_handle_connection_error_while_connecting_to_peers()
        {
            // Arrange 
            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));
            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock.Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromException<IPeer>(new Exception("The network error")));
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();

            // Asset
            peerFactoryMock.Verify(x => x.ConnectTo(It.IsAny<EndPoint>()), Times.Once);
        }

        [TestMethod]
        public void Can_listen_for_peers_on_start()
        {
            // Arrange 
            AutoMockContainer.Register(GetNetworkConfig());
            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();

            // Asset
            peerListenerMock.Verify(x => x.Start(), Times.Once);
        }

        [TestMethod]
        public void Can_stop_listening_for_peers_on_stop()
        {
            // Arrange 
            AutoMockContainer.Register(GetNetworkConfig());
            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();
            server.Stop();

            // Asset
            peerListenerMock.Verify(x => x.Stop(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void Can_disconnect_to_peers_on_stop()
        {
            // Arrange 
            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            FakeHandshake(peerMock);

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock.Setup(x => x.ConnectTo(It.IsAny<EndPoint>())).Returns(Task.FromResult(peerMock.Object));
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();
            WaitUntilPeersAreConnected(server);
            server.Stop();

            // Asset
            peerMock.Verify(x => x.Disconnect(), Times.Once);
        }

        [TestMethod]
        public void Can_stop_server_on_dispose()
        {
            // Arrange 
            AutoMockContainer.Register(GetNetworkConfig());
            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();
            server.Dispose();

            // Asset
            peerListenerMock.Verify(x => x.Stop(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void Can_handle_handshake_different_once_error_while_connecting_to_peer()
        {
            // Arrange 
            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock.Setup(x => x.Receive<VersionMessage>())
                .Returns(() =>
                {
                    var peerVersionMessage = new VersionMessage();
                    peerVersionMessage.Payload.Nonce = 0;
                    return Task.FromResult(peerVersionMessage);
                });

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock.Setup(x => x.ConnectTo(It.IsAny<EndPoint>())).Returns(Task.FromResult(peerMock.Object));
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();
            Task.Delay(100).Wait();

            // Asset
            peerMock.Verify(x => x.Send(new VerAckMessage()), Times.Never);
        }

        private static NetworkConfig GetNetworkConfig(params string[] peerEndPoints)
        {
            var initialData = new Dictionary<string, string>
            {
                { "network:port", "8000" },
                { "network:forceIPv6", "false" },
            };

            for (var i = 0; i < peerEndPoints.Length; i++)
            {
                initialData.Add($"network:peerEndPoints:{i}", peerEndPoints[i]);
            }

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData)
                .Build();

            return new NetworkConfig(configuration);
        }

        private static void FakeHandshake(Mock<IPeer> peerMock)
        {
            var versionMessage = default(VersionMessage);

            peerMock.Setup(x => x.Send(It.IsAny<VersionMessage>()))
                .Callback<Message>(msg => versionMessage = (VersionMessage)msg)
                .Returns(Task.CompletedTask);

            peerMock.Setup(x => x.Receive<VersionMessage>())
                .Returns(() => Task.FromResult(versionMessage));

            var verAckMessage = new VerAckMessage();

            peerMock.Setup(x => x.Send(new VerAckMessage()))
                .Returns(Task.CompletedTask);

            peerMock.Setup(x => x.Receive<VerAckMessage>())
                .Returns(Task.FromResult(verAckMessage));
        }

        private static void WaitUntilPeersAreConnected(IServer server, int timeout = 1000)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.CancelAfter(timeout);

                Task.Run(async () =>
                {
                    while (server.ConnectedPeers.Count == 0)
                    {
                        await Task.Delay(10);
                    }
                }).Wait(cancellationTokenSource.Token);
            }
        }
    }
}