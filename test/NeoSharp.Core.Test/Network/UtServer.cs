using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Core.Network.Security;
using NeoSharp.TestHelpers;
using EndPoint = NeoSharp.Core.Network.EndPoint;

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

            var blockchainContextMock = AutoMockContainer.GetMock<IBlockchainContext>();

            blockchainContextMock
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

            var serverContextMock = this.AutoMockContainer.GetMock<IServerContext>();
            serverContextMock
                .SetupGet(x => x.ConnectedPeers)
                .Returns(new ConcurrentBag<IPeer>());

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

            var serverContextMock = this.AutoMockContainer.GetMock<IServerContext>();
            serverContextMock
                .SetupGet(x => x.ConnectedPeers)
                .Returns(new ConcurrentBag<IPeer>());

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

            var serverContextMock = this.AutoMockContainer.GetMock<IServerContext>();
            serverContextMock
                .SetupGet(x => x.ConnectedPeers)
                .Returns(new ConcurrentBag<IPeer>());

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
            var peerMock = new Mock<IPeer>();
            peerMock
                .SetupGet(x => x.EndPoint)
                .Returns(_peerEndPoint);

            this.AutoMockContainer
                .GetMock<IServerContext>()
                .SetupGet(x => x.ConnectedPeers)
                .Returns(new ConcurrentBag<IPeer> {peerMock.Object});

            var server = AutoMockContainer.Create<Server>();
            var message = new Message();

            // Act
            server.Broadcast(message, peerMock.Object);

            // Assert
            peerMock.Verify(x => x.Send(message), Times.Never);
        }

        [TestMethod]
        public void SendBroadcast_PeerIsNotTheSameAsSource_MessageSendToPeer()
        {
            // Arrange
            var peerMockSource = new Mock<IPeer>();
            peerMockSource
                .SetupGet(x => x.EndPoint)
                .Returns(new EndPoint(Protocol.Tcp, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000)));

            var peerMockNotSource = new Mock<IPeer>();
            peerMockNotSource
                .SetupGet(x => x.EndPoint)
                .Returns(new EndPoint(Protocol.Tcp, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8001)));

            this.AutoMockContainer
                .GetMock<IServerContext>()
                .SetupGet(x => x.ConnectedPeers)
                .Returns(new ConcurrentBag<IPeer> { peerMockSource.Object, peerMockNotSource.Object });

            var server = AutoMockContainer.Create<Server>();
            var message = new Message();

            // Act
            server.Broadcast(message, peerMockSource.Object);

            // Assert
            peerMockSource.Verify(x => x.Send(message), Times.Never);
            peerMockNotSource.Verify(x => x.Send(message), Times.Once);
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