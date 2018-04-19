using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Network;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UtServer : TestBase
    {
        class NullPeer : IPeer, IDisposable
        {
            public void Connect(uint serverNonce)
            {
            }

            public void Disconnect()
            {
            }

            public void Dispose()
            {
            }
        }

        [TestMethod]
        public void Can_connect_to_peers_on_start()
        {
            // Arrange 
            AutoMockContainer.Register(GetNetworkConfig(new[] { "tcp://localhost:8081" }));
            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock.Setup(x => x.Create(It.IsAny<EndPoint>())).Returns(Task.FromResult<IPeer>(new NullPeer()));
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();

            // Asset
            peerFactoryMock.Verify(x => x.Create(It.IsAny<EndPoint>()), Times.Once);
        }

        [TestMethod]
        public void Can_listen_for_peers_on_start()
        {
            // Arrange 
            AutoMockContainer.Register(GetNetworkConfig(new string[0]));
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
            AutoMockContainer.Register(GetNetworkConfig(new string[0]));
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
            AutoMockContainer.Register(GetNetworkConfig(new[] { "tcp://localhost:8081" }));
            AutoMockContainer.Register<IPeer, NullPeer>();
            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerFactoryMock.Setup(x => x.Create(It.IsAny<EndPoint>())).Returns(Task.FromResult(peerMock.Object));
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();
            server.Stop();

            // Asset
            peerMock.Verify(x => x.Disconnect(), Times.Once);
        }

        private static NetworkConfig GetNetworkConfig(string[] peerEndPoints)
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
    }
}