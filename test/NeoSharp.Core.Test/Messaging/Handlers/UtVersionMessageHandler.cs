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
                Version = new VersionPayload()
                {
                    Version = 1,
                    Services = 1,
                    UserAgent = nameof(NullServer),
                    Nonce = (uint)new Random(Environment.TickCount).Next(int.MaxValue),
                };
            }

            public void Start() { }
            public void Stop() { }

            public IReadOnlyCollection<IPeer> ConnectedPeers { get; set; }
            public VersionPayload Version { get; set; }
        }

        [TestMethod]
        public void Throw_on_the_same_nonce_on_version_receiving()
        {
            // Arrange
            var server = new NullServer();
            AutoMockContainer.Register<IServer>(server);

            var versionMessage = GetPeerVersionMessage(server);

            versionMessage.Payload.Nonce = server.Version.Nonce;

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

            server.Version.Version = 2;
            versionMessage.Payload.Version = 1;

            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock.SetupProperty(x => x.Version);

            // Act
            await messageHandler.Handle(versionMessage, peerMock.Object);

            // Assert
            peerMock.Verify(x => x.ChangeProtocol(It.IsAny<VersionPayload>()), Times.Once);
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
            var payload = server.Version;

            return new VersionMessage
            {
                Payload =
                {
                    Nonce = payload.Nonce + 1,
                    Version = payload.Version,
                    UserAgent=payload.UserAgent,
                    Services=payload.Services,
                }
            };
        }
    }
}