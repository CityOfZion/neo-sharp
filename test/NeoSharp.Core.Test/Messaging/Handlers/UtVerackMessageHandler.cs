using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Messaging.Handlers;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Messaging.Handlers
{
    [TestClass]
    public class UtVerAckMessageHandler : TestBase
    {
        [TestMethod]
        public async Task Handle_VerAckMessageReceived_PeerIsReady()
        {
            // Arrange
            var verAckMessage = new VerAckMessage();
            var version = new VersionPayload
            {
                CurrentBlockIndex = 1
            };
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock.SetupProperty(x => x.IsReady, false);
            peerMock.SetupProperty(x => x.Version, version);

            var expectedLastBlockHeader = new BlockHeader(HeaderType.Header)
            {
                Index = 1
            };

            this.AutoMockContainer
                .GetMock<IBlockchainContext>()
                .SetupGet(x => x.LastBlockHeader)
                .Returns(expectedLastBlockHeader);

            var messageHandler = AutoMockContainer.Get<VerAckMessageHandler>();

            // Act
            await messageHandler.Handle(verAckMessage, peerMock.Object);

            // Assert
            peerMock.Object.IsReady
                .Should()
                .BeTrue();
        }

        [TestMethod]
        public async Task Handle_LastLocalBlockIndex1LastPeerBlockIndex2_GetBlockMessageSend()
        {
            // Arrange
            const uint currentBlockIndex = 2;

            var blockHeader = new BlockHeader
            {
                Index = 1,
                Hash = UInt256.Zero
            };

            var blockchainContextMock = this.AutoMockContainer.GetMock<IBlockchainContext>();
            blockchainContextMock
                .SetupGet(x => x.LastBlockHeader)
                .Returns(blockHeader);
            blockchainContextMock
                .SetupGet(x => x.NeedPeerSync)
                .Returns(true);

            var verAckMessage = new VerAckMessage();
            var version = new VersionPayload();
            var peerMock = AutoMockContainer.GetMock<IPeer>();

            peerMock.SetupProperty(x => x.Version, version);
            version.CurrentBlockIndex = currentBlockIndex;

            var messageHandler = AutoMockContainer.Get<VerAckMessageHandler>();

            // Act
            await messageHandler.Handle(verAckMessage, peerMock.Object);

            // Assert
            blockchainContextMock.Verify(x => x.SetPeerCurrentBlockIndex(currentBlockIndex));
            // TODO #410: This need to evaluate correctly that the right GetBlockHeadersMessage is been generated.
            peerMock.Verify(x => x.Send(It.IsAny<GetBlockHeadersMessage>()), Times.Once);
        }

        // TODO #435: There aren't tests for the case of the node has more blocks then the peer node.
    }
}