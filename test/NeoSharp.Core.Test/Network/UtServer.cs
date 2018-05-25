using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Logging;
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

        // TODO: This test is test nothing and will be refactor
        //[TestMethod]
        //public void Can_handle_handshake_different_once_error_while_connecting_to_peer()
        //{
        //    // Arrange 
        //    var waitPeerIsConnectedResetEvent = new AutoResetEvent(false);

        //    AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

        //    var blockchainMock = this.AutoMockContainer.GetMock<IBlockchain>();
        //    blockchainMock
        //        .SetupGet(x => x.CurrentBlock)
        //        .Returns(new Block());

        //    var peerMock = AutoMockContainer.GetMock<IPeer>();
        //    peerMock
        //        .Setup(x => x.Receive())
        //        .Returns(() =>
        //        {
        //            var peerVersionMessage = new VersionMessage {Payload = {Nonce = 0}};
        //            return Task.FromResult((Message) peerVersionMessage);
        //        });

        //    peerMock
        //        .SetupGet(x => x.IsConnected)
        //        .Returns(true);

        //    //peerMock
        //    //    .Setup(x => x.Send(It.IsAny<VersionMessage>()))
        //    //    .Callback(() => waitPeerIsConnectedResetEvent.Set());

        //    var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
        //    peerFactoryMock
        //        .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
        //        .Returns(Task.FromResult(peerMock.Object));

        //    // Act
        //    var server = AutoMockContainer.Create<Server>();
        //    server.Start();
        //    Task.Delay(100).Wait();

        //    //waitPeerIsConnectedResetEvent.WaitOne();

        //    // Asset
        //    peerMock.Verify(x => x.Send(new VerAckMessage()), Times.Never);
        //}

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