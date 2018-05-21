using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Messaging.Handlers;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Messaging.Handlers
{
    [TestClass]
    public class UtVersionMessageHandler : TestBase
    {
        class NullServer : IServer
        {
            public NullServer()
            {
                ProtocolVersion = 1;
                Nonce = (uint)new Random(Environment.TickCount).Next(int.MaxValue);
            }

            public void Start()
            {
            }

            public void Stop()
            {
            }

            public IReadOnlyCollection<IPeer> ConnectedPeers { get; set; }

            public uint ProtocolVersion { get; set; }

            public uint Nonce { get; }
        }

        [TestMethod]
        public void Throw_on_the_same_nonce_on_version_receiving()
        {
            // Arrange
            var server = new NullServer();
            AutoMockContainer.Register<IServer>(server);

            var versionMessage = GetPeerVersionMessage(server);

            versionMessage.Payload.Nonce = server.Nonce;

            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock.SetupProperty(x => x.Version);

            var messageHandler = AutoMockContainer.Get<VersionMessageHandler>();

            // Act
            Action a = () => messageHandler.Handle(versionMessage, peerMock.Object).Wait();

            // Assert
            a.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public async Task Can_downgrade_protocol_on_version_receiving()
        {
            // Arrange
            var server = new NullServer();
            AutoMockContainer.Register<IServer>(server);

            var messageHandler = AutoMockContainer.Get<VersionMessageHandler>();
            var versionMessage = GetPeerVersionMessage(server);

            server.ProtocolVersion = 2;
            versionMessage.Payload.Version = 1;

            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock.SetupProperty(x => x.Version);

            // Act
            await messageHandler.Handle(versionMessage, peerMock.Object);

            // Assert
            peerMock.Verify(x => x.DowngradeProtocol(It.IsAny<uint>()), Times.Once);
        }


        [TestMethod]
        public async Task Can_send_acknowledgement_on_version_receiving()
        {
            // Arrange
            var server = new NullServer();
            AutoMockContainer.Register<IServer>(server);

            var messageHandler = AutoMockContainer.Get<VersionMessageHandler>();
            var versionMessage = GetPeerVersionMessage(server);
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock.SetupProperty(x => x.Version);

            // Act
            await messageHandler.Handle(versionMessage, peerMock.Object);

            // Assert
            peerMock.Verify(x => x.Send<VerAckMessage>(), Times.Once);
        }

        private static VersionMessage GetPeerVersionMessage(IServer server)
        {
            return new VersionMessage
            {
                Payload =
                {
                    Nonce = server.Nonce + 1,
                    Version = server.ProtocolVersion
                }
            };
        }
    }
}