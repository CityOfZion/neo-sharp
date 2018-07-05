using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UtServer : TestBase
    {
        private EndPoint _peerEndPoint;

        [TestInitialize]
        public void Initialize()
        {
            var networkConfig = GetNetworkConfig("tcp://localhost:8081");
            _peerEndPoint = networkConfig.PeerEndPoints[0];

            AutoMockContainer.Register(networkConfig);

            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();

            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());
        }

        [TestMethod]
        public void Start_ValidNetworkConfiguration_ConnectToPeerAsStartListeningForPeers()
        {
            // Arrange 
            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();
            var peerMessageListenerMock = AutoMockContainer.GetMock<IPeerMessageListener>();
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock
                .SetupGet(x => x.EndPoint)
                .Returns(_peerEndPoint);

            var peer = peerMock.Object;
            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();

            peerFactoryMock
                .Setup(x => x.ConnectTo(_peerEndPoint))
                .Returns(Task.FromResult(peer));

            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();

            // Assert
            peerFactoryMock.Verify(x => x.ConnectTo(_peerEndPoint), Times.Once);
            peerListenerMock.Verify(x => x.Start(), Times.Once);
            peerMessageListenerMock.Verify(x => x.StartFor(peer, It.IsAny<CancellationToken>()), Times.Once);
        }

        [TestMethod]
        public void Start_PeerConnectionThrowException_WarningMessageIsLoggedServerKeepListeningForPeers()
        {
            // Arrange             
            var connectionException = new Exception("The network error");
            var expectedLoggedWarningMessage = $"Something went wrong with {_peerEndPoint}. Exception: {connectionException}";

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();

            peerFactoryMock
                .Setup(x => x.ConnectTo(_peerEndPoint))
                .Returns(Task.FromException<IPeer>(connectionException));

            var loggerMock = AutoMockContainer.GetMock<ILogger<Server>>();
            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();

            // Assert
            peerFactoryMock.Verify(x => x.ConnectTo(_peerEndPoint), Times.Once);
            loggerMock.Verify(x => x.LogWarning(It.Is<string>(msg => msg.Contains(expectedLoggedWarningMessage))), Times.Once);
            peerListenerMock.Verify(x => x.Start(), Times.Once);        
        }

        [TestMethod]
        public void Stop_SuccessfulPeerConnection_StoppingServerLeadsToDisconnectingPeer()
        {
            // Arrange 
            var peerMessageListenerMock = AutoMockContainer.GetMock<IPeerMessageListener>();
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock
                .SetupGet(x => x.EndPoint)
                .Returns(_peerEndPoint);

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();

            peerFactoryMock
                .Setup(x => x.ConnectTo(_peerEndPoint))
                .Returns(Task.FromResult(peerMock.Object));

            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();
            server.Stop();

            // Assert
            peerListenerMock.Verify(x => x.Start(), Times.Once);
            peerMessageListenerMock.Verify(x => x.StartFor(peerMock.Object, It.IsAny<CancellationToken>()), Times.Once);
            peerMock.Verify(x => x.Disconnect(), Times.Once);
            peerListenerMock.Verify(x => x.Stop(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void Dispose_ServerIsRunning_StopListenerAndDisconnectPeer()
        {
            // Arrange 
            var peerMessageListenerMock = AutoMockContainer.GetMock<IPeerMessageListener>();
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock
                .SetupGet(x => x.EndPoint)
                .Returns(_peerEndPoint);

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();

            peerFactoryMock
                .Setup(x => x.ConnectTo(_peerEndPoint))
                .Returns(Task.FromResult(peerMock.Object));

            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();
            var server = AutoMockContainer.Create<Server>();

            // Act
            server.Start();
            server.Dispose();

            // Assert
            peerListenerMock.Verify(x => x.Start(), Times.Once);
            peerMessageListenerMock.Verify(x => x.StartFor(peerMock.Object, It.IsAny<CancellationToken>()), Times.Once);
            peerMock.Verify(x => x.Disconnect(), Times.Once);
            peerListenerMock.Verify(x => x.Stop(), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task SendBroadcast_FilterIsNull_MessageSendToConnectedPeers()
        {
            // Arrange
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock
                .SetupGet(x => x.EndPoint)
                .Returns(_peerEndPoint);

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();

            peerFactoryMock
                .Setup(x => x.ConnectTo(_peerEndPoint))
                .Returns(Task.FromResult(peerMock.Object));

            var server = AutoMockContainer.Create<Server>();
            var message = new Message();

            // Act
            server.Start();

            await server.SendBroadcast(message);

            // Assert
            peerMock.Verify(x => x.Send(message), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task SendBroadcast_FilterEqualFalse_MessageNotSendToBroadcaster()
        {
            // Arrange
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock
                .SetupGet(x => x.EndPoint)
                .Returns(_peerEndPoint);

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();

            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            var server = AutoMockContainer.Create<Server>();
            var message = new Message();

            // Act
            server.Start();

            await server.SendBroadcast(message, peer => false);

            // Assert
            peerMock.Verify(x => x.Send(message), Times.Never);
        }

        [TestMethod]
        public async Task SendBroadcast_FilterEqualTrue_MessageSendToPeer()
        {
            // Arrange
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock
                .SetupGet(x => x.EndPoint)
                .Returns(_peerEndPoint);

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();

            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            var server = AutoMockContainer.Create<Server>();
            var message = new Message();

            // Act
            server.Start();

            await server.SendBroadcast(message, peer => true);

            // Assert
            peerMock.Verify(x => x.Send(message), Times.Once);
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

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData)
                .Build();

            return new NetworkConfig(config);
        }
    }
}