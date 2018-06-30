using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Network;
using NeoSharp.Core.Network.Tcp;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UtPeerFactory : TestBase
    {
        [TestMethod]
        public void Can_resolve_for_known_protocols()
        {
            // Arrange 
            var tcpPeerFactoryMock = AutoMockContainer.GetMock<ITcpPeerFactory>();
            var peerFactory = AutoMockContainer.Create<PeerFactory>();
            var endPoint = new EndPoint { Protocol = Protocol.Tcp };

            // Act
            peerFactory.ConnectTo(endPoint);

            // Assert
            tcpPeerFactoryMock.Verify(x => x.ConnectTo(It.IsAny<EndPoint>()), Times.Once);
        }

        [TestMethod]
        public void Throw_when_endpoint_is_null()
        {
            // Arrange 
            var peerFactory = AutoMockContainer.Create<PeerFactory>();

            // Act
            Action a = () => peerFactory.ConnectTo(null);

            // Assert
            a.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Throw_when_resolving_for_unknown_protocols()
        {
            // Arrange 
            var peerFactory = AutoMockContainer.Create<PeerFactory>();
            var endPoint = new EndPoint { Protocol = Protocol.Unknown };

            // Act
            Action a = () => peerFactory.ConnectTo(endPoint);

            // Assert
            a.Should().Throw<NotSupportedException>();
        }
    }
}