using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UtPeerMessageListener : TestBase
    {
        //[TestMethod]
        //public void ListenerMessagesFromPeer_PeerIsReadyAndMessageIsNotHandshake_MessageIsHandled()
        //{
        //    // Arrange
        //    var waitNextPeerConnectionLoopResetEvent = new ManualResetEvent(false);

        //    AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

        //    var messageHandlerMock = this.AutoMockContainer.GetMock<IMessageHandler<Message>>();

        //    var blockchainMock = this.AutoMockContainer.GetMock<IBlockchain>();
        //    blockchainMock
        //        .SetupGet(x => x.CurrentBlock)
        //        .Returns(new Block());

        //    var peerMessage = new Message();
        //    var peerMock = AutoMockContainer.GetMock<IPeer>();
        //    peerMock
        //        .Setup(x => x.Receive())
        //        .Returns(() => Task.FromResult(peerMessage));
        //    peerMock
        //        .SetupGet(x => x.IsReady)
        //        .Returns(true);

        //    peerMock
        //        .SetupGet(x => x.IsConnected)
        //        .Returns(true);

        //    var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
        //    peerFactoryMock
        //        .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
        //        .Returns(Task.FromResult(peerMock.Object));

        //    // Act
        //    var server = AutoMockContainer.Create<Server>();
        //    server.Start();
        //    server.Stop();

        //    // Assert
        //    peerMock.Verify(x => x.Send(new VerAckMessage()), Times.Never);
        //    peerMock.Verify(x => x.Send(It.IsAny<VersionMessage>()), Times.Once);
        //    peerMock.Verify(x => x.Receive());
        //    messageHandlerMock.Verify(x => x.Handle(peerMessage, peerMock.Object));
        //}

        //[TestMethod]
        //public void ListenerMessagesFromPeer_PeerIsReadyAndMessageIsHandshake_MessageIsNotHandled()
        //{
        //    // Arrange 
        //    var waitPeerIsReadyResetEvent = new ManualResetEvent(false);

        //    AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

        //    var messageHandlerMock = this.AutoMockContainer.GetMock<IMessageHandler<Message>>();

        //    var blockchainMock = this.AutoMockContainer.GetMock<IBlockchain>();
        //    blockchainMock
        //        .SetupGet(x => x.CurrentBlock)
        //        .Returns(new Block());

        //    var asyncDelayerMock = this.AutoMockContainer.GetMock<IAsyncDelayer>();

        //    var serverContextMock = AutoMockContainer
        //        .GetMock<IServerContext>()
        //        .SetupDefaultServerContext();

        //    var peerMessage = new VersionMessage(serverContextMock.Object.Version);
        //    var peerMock = AutoMockContainer.GetMock<IPeer>();
        //    peerMock
        //        .Setup(x => x.Receive())
        //        .Returns(() => Task.FromResult((Message)peerMessage));

        //    peerMock
        //        .SetupGet(x => x.IsReady)
        //        .Returns(true)
        //        .Callback(() =>
        //        {
        //            waitPeerIsReadyResetEvent.Set();
        //        });

        //    peerMock
        //        .SetupGet(x => x.IsConnected)
        //        .Returns(true);

        //    var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
        //    peerFactoryMock
        //        .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
        //        .Returns(Task.FromResult(peerMock.Object));

        //    // Act
        //    var server = AutoMockContainer.Create<Server>();
        //    server.Start();

        //    var waitTimedOut = waitPeerIsReadyResetEvent.WaitOne(TimeSpan.FromSeconds(3));

        //    server.Stop();

        //    // Assert
        //    waitTimedOut.Should().BeTrue();
        //    peerMock.Verify(x => x.Send(new VerAckMessage()), Times.Never);
        //    peerMock.Verify(x => x.Send(It.IsAny<VersionMessage>()), Times.Once);
        //    peerMock.Verify(x => x.Receive());
        //    messageHandlerMock.Verify(x => x.Handle(peerMessage, peerMock.Object), Times.Never);
        //    asyncDelayerMock.Verify(x => x.Delay(TimeSpan.FromSeconds(1), It.IsAny<CancellationToken>()), Times.Never);
        //}

        //[TestMethod]
        //public void ListenerMessagesFromPeer_PeerIsNotReadyAndMessageIsNotHandshake_MessageIsNotHandled()
        //{
        //    // Arrange 
        //    var waitPeerIsReadyResetEvent = new ManualResetEvent(false);

        //    AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

        //    var messageHandlerMock = this.AutoMockContainer.GetMock<IMessageHandler<Message>>();

        //    var blockchainMock = this.AutoMockContainer.GetMock<IBlockchain>();
        //    blockchainMock
        //        .SetupGet(x => x.CurrentBlock)
        //        .Returns(new Block());

        //    var peerMessage = new Message();
        //    var peerMock = AutoMockContainer.GetMock<IPeer>();
        //    peerMock
        //        .Setup(x => x.Receive())
        //        .Returns(() => Task.FromResult(peerMessage));

        //    peerMock
        //        .SetupGet(x => x.IsReady)
        //        .Returns(false)
        //        .Callback(() =>
        //        {
        //            waitPeerIsReadyResetEvent.Set();
        //        });

        //    peerMock
        //        .SetupGet(x => x.IsConnected)
        //        .Returns(true);

        //    var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
        //    peerFactoryMock
        //        .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
        //        .Returns(Task.FromResult(peerMock.Object));

        //    // Act
        //    var server = AutoMockContainer.Create<Server>();
        //    server.Start();

        //    var waitTimedOut = waitPeerIsReadyResetEvent.WaitOne(TimeSpan.FromSeconds(3));

        //    server.Stop();

        //    // Assert
        //    waitTimedOut.Should().BeTrue();
        //    peerMock.Verify(x => x.Send(new VerAckMessage()), Times.Never);
        //    peerMock.Verify(x => x.Send(It.IsAny<VersionMessage>()), Times.Once);
        //    peerMock.Verify(x => x.Receive());
        //    messageHandlerMock.Verify(x => x.Handle(peerMessage, peerMock.Object), Times.Never);
        //}

        //[TestMethod]
        //public void ListenerMessagesFromPeer_PeerIsNotReadyAndMessageIsHandshake_MessageIsHandled()
        //{
        //    // Arrange 
        //    var waitPeerIsReadyResetEvent = new ManualResetEvent(false);

        //    AutoMockContainer.Register(GetNetworkConfig("tcp://localhost:8081"));

        //    var messageHandlerMock = this.AutoMockContainer.GetMock<IMessageHandler<Message>>();

        //    var blockchainMock = this.AutoMockContainer.GetMock<IBlockchain>();
        //    blockchainMock
        //        .SetupGet(x => x.CurrentBlock)
        //        .Returns(new Block());

        //    var serverContextMock = AutoMockContainer
        //        .GetMock<IServerContext>()
        //        .SetupDefaultServerContext();

        //    var peerMessage = new VersionMessage(serverContextMock.Object.Version);
        //    var peerMock = AutoMockContainer.GetMock<IPeer>();
        //    peerMock
        //        .Setup(x => x.Receive())
        //        .Returns(() => Task.FromResult((Message)peerMessage));

        //    peerMock
        //        .SetupGet(x => x.IsReady)
        //        .Returns(false)
        //        .Callback(() =>
        //        {
        //            waitPeerIsReadyResetEvent.Set();
        //        });

        //    peerMock
        //        .SetupGet(x => x.IsConnected)
        //        .Returns(true);

        //    var peerFactoryMock = AutoMockContainer.GetMock<IPeerFactory>();
        //    peerFactoryMock
        //        .Setup(x => x.ConnectTo(It.IsAny<EndPoint>()))
        //        .Returns(Task.FromResult(peerMock.Object));

        //    // Act
        //    var server = AutoMockContainer.Create<Server>();
        //    server.Start();

        //    var waitTimedOut = waitPeerIsReadyResetEvent.WaitOne(TimeSpan.FromSeconds(3));

        //    server.Stop();

        //    // Assert
        //    waitTimedOut.Should().BeTrue();
        //    peerMock.Verify(x => x.Send(new VerAckMessage()), Times.Never);
        //    peerMock.Verify(x => x.Send(It.IsAny<VersionMessage>()), Times.Once);
        //    peerMock.Verify(x => x.Receive());
        //    messageHandlerMock.Verify(x => x.Handle(peerMessage, peerMock.Object));
        //}
    }
}
