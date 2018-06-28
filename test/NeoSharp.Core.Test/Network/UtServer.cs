using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain;
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
            const Protocol expectedProtocol = Protocol.Tcp;
            const string expectedHost = "localhost";
            const string expectedPort = "8081";

            AutoMockContainer.Register(new [] {$"tcp://{expectedHost}:{expectedPort}"}.GetNetworkConfig());

            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());

            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();

            var peerMessageListenerMock = AutoMockContainer.GetMock<IPeerMessageListener>();

            var peerMock = AutoMockContainer.GetMock<IPeer>();

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            // Asset
            peerFactoryMock.Verify(x => x.ConnectTo(It.Is<EndPoint>(ep => ep.Host == "localhost" && ep.Port == 8081 && ep.Protocol == expectedProtocol)), Times.Once);
            peerListenerMock.Verify(x => x.Start(), Times.Once);
            peerMessageListenerMock.Verify(x => x.StartListen(peerMock.Object), Times.Once);
            peerMock.Verify(x => x.Send(It.IsAny<VersionMessage>()), Times.Once);
        }

        [TestMethod]
        public void Start_PeerConnectionThrowException_WarningMessageIsLoggedServerKeepListeningForPeers()
        {
            // Arrange 
            const Protocol expectedProtocol = Protocol.Tcp;
            const string expectedHost = "localhost";
            const string expectedPort = "8081";
            var peerEndPoint = $"tcp://{expectedHost}:{expectedPort}";

            AutoMockContainer.Register(new [] {peerEndPoint}.GetNetworkConfig());
            
            var connectionException = new Exception("The network error");
            var expectedLoggedWarningMessage = $"Something went wrong with {peerEndPoint}. Exception: {connectionException}";

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
            peerFactoryMock.Verify(x => x.ConnectTo(It.Is<EndPoint>(ep => ep.Host == "localhost" && ep.Port == 8081 && ep.Protocol == expectedProtocol)), Times.Once);
            loggerMock.Verify(x => x.LogWarning(It.Is<string>(msg => msg.Contains(expectedLoggedWarningMessage))), Times.Once);

            peerListenerMock.Verify(x => x.Start(), Times.Once);        
        }

        [TestMethod]
        public void Stop_SuccessfulPeerConnection_StoppingServerLeadsToDisconnectingPeer()
        {
            // Arrange 
            var waitSendToPeerVersionMessageResetEvent = new ManualResetEvent(false);

            AutoMockContainer.Register(new[] {"tcp://localhost:8081"}.GetNetworkConfig());

            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());

            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .SetupFakeHandshake()
                .Setup(x => x.Send(It.IsAny<VersionMessage>()))
                .Callback(() => waitSendToPeerVersionMessageResetEvent.Set());

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            waitSendToPeerVersionMessageResetEvent.WaitOne();

            server.Stop();

            // Asset
            peerMock.Verify(x => x.Disconnect(), Times.Once);
            peerListenerMock.Verify(x => x.Stop(), Times.AtLeastOnce);
        }

        [TestMethod]
        public void Dispose_ServerIsRunning_StopListenerAndDisconnectPeer()
        {
            // Arrange 
            var waitSendToPeerVersionMessageResetEvent = new ManualResetEvent(false);

            AutoMockContainer.Register(new [] {"tcp://localhost:8081"}.GetNetworkConfig());

            var blockchainMock = AutoMockContainer.GetMock<IBlockchain>();
            blockchainMock
                .SetupGet(x => x.CurrentBlock)
                .Returns(new Block());

            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .SetupFakeHandshake()
                .Setup(x => x.Send(It.IsAny<VersionMessage>()))
                .Callback(() => waitSendToPeerVersionMessageResetEvent.Set());

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            var peerListenerMock = AutoMockContainer.GetMock<IPeerListener>();

            // Act
            var server = AutoMockContainer.Create<Server>();
            server.Start();

            waitSendToPeerVersionMessageResetEvent.WaitOne();

            server.Dispose();

            // Asset
            peerMock.Verify(x => x.Disconnect(), Times.Once);
            peerListenerMock.Verify(x => x.Stop(), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task SendBroadcast_FilterIsNull_MessageSendToConnectedPeers()
        {
            var waitSendToPeerVersionMessageResetEvent = new ManualResetEvent(false);
            var message = new Message();

            AutoMockContainer.Register(new[] { "tcp://localhost:8081" }.GetNetworkConfig());

            var server = AutoMockContainer.Create<Server>();
            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .SetupFakeHandshake()
                .Setup(x => x.Send(It.IsAny<VersionMessage>()))
                .Callback(() => waitSendToPeerVersionMessageResetEvent.Set());

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            server.Start();
            waitSendToPeerVersionMessageResetEvent.WaitOne();

            await server.SendBroadcast(message, null);

            peerMock.Verify(x => x.Send(message));
        }

        [TestMethod]
        public async Task SendBroadcast_FilterEqualFalse_MessageNotSendToBroadcaster()
        {
            var waitSendToPeerVersionMessageResetEvent = new ManualResetEvent(false);
            var message = new Message();

            AutoMockContainer.Register(new[] { "tcp://localhost:8081" }.GetNetworkConfig());

            var server = AutoMockContainer.Create<Server>();
            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .SetupFakeHandshake()
                .Setup(x => x.Send(It.IsAny<VersionMessage>()))
                .Callback(() => waitSendToPeerVersionMessageResetEvent.Set());

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            server.Start();
            waitSendToPeerVersionMessageResetEvent.WaitOne();

            await server.SendBroadcast(message, peer => false);

            peerMock.Verify(x => x.Send(message), Times.Never);
        }

        [TestMethod]
        public async Task SendBroadcast_FilterEqualTrue_MessageSendToPeer()
        {
            var waitSendToPeerVersionMessageResetEvent = new ManualResetEvent(false);
            var message = new Message();

            AutoMockContainer.Register(new[] { "tcp://localhost:8081" }.GetNetworkConfig());

            var server = AutoMockContainer.Create<Server>();
            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .SetupFakeHandshake()
                .Setup(x => x.Send(It.IsAny<VersionMessage>()))
                .Callback(() => waitSendToPeerVersionMessageResetEvent.Set());

            var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
            peerFactoryMock
                .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
                .Returns(Task.FromResult(peerMock.Object));

            server.Start();
            waitSendToPeerVersionMessageResetEvent.WaitOne();

            await server.SendBroadcast(message, peer => true);

            peerMock.Verify(x => x.Send(message), Times.Once);
        }
    }
}