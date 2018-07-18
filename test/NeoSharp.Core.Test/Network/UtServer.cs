using System;
using System.IO;
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
using NeoSharp.Core.Network.Security;
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
            var networkConfig = GetNetworkConfig();
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
        public void Broadcast_PeerIsTheSameAsSource_MessageNotSendToPeer()
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
            server.Broadcast(message, peerMock.Object);

            // Assert
            peerMock.Verify(x => x.Send(message), Times.Never);
        }

        [TestMethod]
        public void SendBroadcast_PeerIsNotTheSameAsSource_MessageSendToPeer()
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
            server.Broadcast(message);

            // Assert
            peerMock.Verify(x => x.Send(message), Times.Once);
        }

        [TestMethod]
        public void Start_PeerIsNotAllowed_WarningLoggedAndPeerDisconnected()
        {
            // Arrange 
            var networkAcl = new NetworkAcl(NetworkAclType.Blacklist, new[] { new NetworkAcl.Entry("localhost") } );

            var networkAclLoaderMock = this.AutoMockContainer.GetMock<INetworkAclLoader>();
            networkAclLoaderMock
                .Setup(x => x.Load(It.IsAny<NetworkAclConfig>()))
                .Returns(networkAcl);

            var loggerMock = this.AutoMockContainer.GetMock<ILogger<Server>>();

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
            loggerMock.Verify(x => x.LogWarning(It.Is<string>(s => s.StartsWith("Something went wrong with "))), Times.Once);
            peerMock.Verify(x => x.Disconnect(), Times.Once);
        }

        private static NetworkConfig GetNetworkConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            var configuration = builder.Build();

            return new NetworkConfig(configuration);
        }
    }
}