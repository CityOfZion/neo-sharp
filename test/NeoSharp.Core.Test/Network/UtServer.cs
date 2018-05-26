using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Core.Test.ExtensionMethods;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UtServer : TestBase
    {
        [TestMethod]
        public void Start_ValidNetworkConfiguration_ConnectToPeerAsStartListeningForPeers()
        {
            // Arrange 
            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());

            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();

            var peerMock = AutoMockContainer.GetMock<IPeer>();

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            // Asset
            peerFactoryMock.Verify(x => x.ConnectTo(It.IsAny<EndPoint>()), Times.Once);
            peerListenerMock.Verify(x => x.Start(), Times.Once);
        }

        [TestMethod]
        public void Start_PeerConnectionThrowException_WarningMessageIsLoggedServerKeepListeningForPeers()
        {
            // Arrange 
            const string peerEndPoint = "tcp://localhost:8081";
            var connectionException = new Exception("The network error");

            AutoMockContainer.Register(GetNetworkConfig(peerEndPoint));

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromException<IPeer>(connectionException));

            var loggerMock = AutoMockContainer.GetMock<ILogger<Server>>();
            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            // Asset
            peerFactoryMock.Verify(x => x.ConnectTo(It.IsAny<EndPoint>()), Times.Once);
            loggerMock.Verify(x => x.LogWarning(It.IsAny<string>()), Times.Once);

            peerListenerMock.Verify(x => x.Start(), Times.Once);
        }

        [TestMethod]
        public void Stop_SuccessfulPeerConnection_StoppingServerLeadsToDisconnectingPeer()
        {
            // Arrange 
            var waitPeerIsConnectedResetEvent = new AutoResetEvent(false);

            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());

            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .SetupFakeHandshake()
                .Setup(x => x.Send(It.IsAny<VersionMessage>()))
                .Callback(() => waitPeerIsConnectedResetEvent.Set());

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            waitPeerIsConnectedResetEvent.WaitOne();

            server.Stop();

            // Asset
            peerMock.Verify(x => x.Disconnect(), Times.Once);
            peerListenerMock.Verify(x => x.Stop(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void Dispose_ServerIsRunning_StopListenerAndDisconnectPeer()
        {
            // Arrange 
            var waitPeerIsConnectedResetEvent = new AutoResetEvent(false);

            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());

            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .SetupFakeHandshake()
                .Setup(x => x.Send(It.IsAny<VersionMessage>()))
                .Callback(() => waitPeerIsConnectedResetEvent.Set());

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            waitPeerIsConnectedResetEvent.WaitOne();

            server.Dispose();

            // Asset
            peerMock.Verify(x => x.Disconnect(), Times.Once);
            peerListenerMock.Verify(x => x.Stop(), Times.AtLeastOnce);

        }

        [TestMethod]
        public void ListenerMessagesFromPeer_PeerIsReadyAndMessageIsNotHandshake_MessageIsHandled()
        {
            // Arrange 
            var waitPeerIsConnectedResetEvent = new AutoResetEvent(false);

            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

            var messageHandlerMock = this.AutoMockContainer.GetMock<IMessageHandler<Message>>();

            var blockchainMock = this.AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());

            var asyncDelayerMock = this.AutoMockContainer.GetMock<IAsyncDelayer>();
            asyncDelayerMock
                .Setup(x => x.Delay(TimeSpan.FromSeconds(1), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(0))
                .Callback(() =>
                {
                    waitPeerIsConnectedResetEvent.Set();
                });

            var peerMessage = new Message();
            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .Setup(x => x.Receive())
                .Returns(() => Task.FromResult(peerMessage));
            peerMock
                .SetupGet(x => x.IsReady)
                .Returns(true);

            peerMock
                .SetupGet(x => x.IsConnected)
                .Returns(true);

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            var waitTimedOut = waitPeerIsConnectedResetEvent.WaitOne();

            server.Stop();

            // Asset
            Assert.IsTrue(waitTimedOut);
            peerMock.Verify(x => x.Send(new VerAckMessage()), Times.Never);
            peerMock.Verify(x => x.Send(It.IsAny<VersionMessage>()), Times.Once);
            peerMock.Verify(x => x.Receive());
            messageHandlerMock.Verify(x => x.Handle(peerMessage, peerMock.Object));
            asyncDelayerMock.Verify(x => x.Delay(TimeSpan.FromSeconds(1), It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public void ListenerMessagesFromPeer_PeerIsReadyAndMessageIsHandshake_MessageIsHandled()
        {
            // Arrange 
            var waitPeerIsConnectedResetEvent = new AutoResetEvent(false);

            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

            var messageHandlerMock = this.AutoMockContainer.GetMock<IMessageHandler<Message>>();

            var blockchainMock = this.AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());

            var asyncDelayerMock = this.AutoMockContainer.GetMock<IAsyncDelayer>();

            var peerMessage = new VersionMessage();
            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .Setup(x => x.Receive())
                .Returns(() => Task.FromResult((Message)peerMessage));

            peerMock
                .SetupGet(x => x.IsReady)
                .Returns(true)
                .Callback(() =>
                {
                    waitPeerIsConnectedResetEvent.Set();
                });

            peerMock
                .SetupGet(x => x.IsConnected)
                .Returns(true);

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            var waitTimedOut = waitPeerIsConnectedResetEvent.WaitOne(TimeSpan.FromSeconds(3));

            server.Stop();

            // Asset
            Assert.IsTrue(waitTimedOut);
            peerMock.Verify(x => x.Send(new VerAckMessage()), Times.Never);
            peerMock.Verify(x => x.Send(It.IsAny<VersionMessage>()), Times.Once);
            peerMock.Verify(x => x.Receive());
            messageHandlerMock.Verify(x => x.Handle(peerMessage, peerMock.Object));
            asyncDelayerMock.Verify(x => x.Delay(TimeSpan.FromSeconds(1), It.IsAny<CancellationToken>()));
        }

        [TestMethod]
        public void ListenerMessagesFromPeer_PeerIsNotReadyAndMessageIsNotHandshake_MessageIsNotHandled()
        {
            // Arrange 
            var waitPeerIsConnectedResetEvent = new AutoResetEvent(false);

            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

            var messageHandlerMock = this.AutoMockContainer.GetMock<IMessageHandler<Message>>();

            var blockchainMock = this.AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());

            var asyncDelayerMock = this.AutoMockContainer.GetMock<IAsyncDelayer>();

            var peerMessage = new Message();
            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .Setup(x => x.Receive())
                .Returns(() => Task.FromResult(peerMessage));

            peerMock
                .SetupGet(x => x.IsReady)
                .Returns(false)
                .Callback(() =>
                {
                    waitPeerIsConnectedResetEvent.Set();
                });

            peerMock
                .SetupGet(x => x.IsConnected)
                .Returns(true);

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            var waitTimedOut = waitPeerIsConnectedResetEvent.WaitOne(TimeSpan.FromSeconds(3));

            server.Stop();

            // Asset
            Assert.IsTrue(waitTimedOut);
            peerMock.Verify(x => x.Send(new VerAckMessage()), Times.Never);
            peerMock.Verify(x => x.Send(It.IsAny<VersionMessage>()), Times.Once);
            peerMock.Verify(x => x.Receive());
            messageHandlerMock.Verify(x => x.Handle(peerMessage, peerMock.Object), Times.Never);
            asyncDelayerMock.Verify(x => x.Delay(TimeSpan.FromSeconds(1), It.IsAny<CancellationToken>()), Times.Never);
        }

        [TestMethod]
        public void ListenerMessagesFromPeer_PeerIsNotReadyAndMessageIsHandshake_MessageIsHandled()
        {
            // Arrange 
            var waitPeerIsConnectedResetEvent = new AutoResetEvent(false);

            AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

            var messageHandlerMock = this.AutoMockContainer.GetMock<IMessageHandler<Message>>();

            var blockchainMock = this.AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());

            var asyncDelayerMock = this.AutoMockContainer.GetMock<IAsyncDelayer>();

            var peerMessage = new VersionMessage();
            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .Setup(x => x.Receive())
                .Returns(() => Task.FromResult((Message)peerMessage));

            peerMock
                .SetupGet(x => x.IsReady)
                .Returns(false)
                .Callback(() =>
                {
                    waitPeerIsConnectedResetEvent.Set();
                });

            peerMock
                .SetupGet(x => x.IsConnected)
                .Returns(true);

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            var waitTimedOut = waitPeerIsConnectedResetEvent.WaitOne(TimeSpan.FromSeconds(3));

            server.Stop();

            // Asset
            Assert.IsTrue(waitTimedOut);
            peerMock.Verify(x => x.Send(new VerAckMessage()), Times.Never);
            peerMock.Verify(x => x.Send(It.IsAny<VersionMessage>()), Times.Once);
            peerMock.Verify(x => x.Receive());
            messageHandlerMock.Verify(x => x.Handle(peerMessage, peerMock.Object));
            asyncDelayerMock.Verify(x => x.Delay(TimeSpan.FromSeconds(1), It.IsAny<CancellationToken>()));
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
    }
}