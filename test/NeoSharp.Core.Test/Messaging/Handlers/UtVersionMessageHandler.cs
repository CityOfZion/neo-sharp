using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Exceptions;
using NeoSharp.Core.Messaging.Handlers;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;
using NeoSharp.Core.Test.ExtensionMethods;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Messaging.Handlers
{
    [TestClass]
    public class UtVersionMessageHandler : TestBase
    {
        [TestMethod]
        public void Throw_on_the_same_nonce_on_version_receiving()
        {
            // Arrange
            var serverContextMock = AutoMockContainer
                .GetMock<IServerContext>()
                .SetupDefaultServerContext();

            var versionMessage = new VersionMessage(serverContextMock.Object.Version);

            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock.SetupProperty(x => x.Version);

            var messageHandler = AutoMockContainer.Get<VersionMessageHandler>();

            // Act
            Action a = () => messageHandler.Handle(versionMessage, peerMock.Object).Wait();

            // Assert
            a.Should().Throw<InvalidMessageException>();
        }

        [TestMethod]
        public async Task Can_downgrade_protocol_on_version_receiving()
        {
            // Arrange
            var serverContextMock = AutoMockContainer
                .GetMock<IServerContext>()
                .SetupDefaultServerContext();

            var versionMessage = new VersionMessage(serverContextMock.Object.Version);

            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .Setup(x => x.ChangeProtocol(peerMock.Object.Version))
                .Returns(false);

            var messageHandler = AutoMockContainer.Get<VersionMessageHandler>();

            // Act
            await messageHandler.Handle(versionMessage, peerMock.Object);

            // Assert
            peerMock.Verify(x => x.ChangeProtocol(It.IsAny<VersionPayload>()), Times.Once);
        }


        [TestMethod]
        public async Task Can_send_acknowledgement_on_version_receiving()
        {
            // Arrange
            var serverContextMock = AutoMockContainer
                .GetMock<IServerContext>()
                .SetupDefaultServerContext();

            var versionMessage = new VersionMessage(serverContextMock.Object.Version);

            var peerMock = AutoMockContainer.GetMock<IPeer>();
            peerMock
                .SetupGet(x => x.Version)
                .Returns(new VersionPayload());

            var messageHandler = AutoMockContainer.Get<VersionMessageHandler>();

            // Act
            await messageHandler.Handle(versionMessage, peerMock.Object);

            // Assert
            peerMock.Verify(x => x.Send<VerAckMessage>(), Times.Once);
        }
    }
}