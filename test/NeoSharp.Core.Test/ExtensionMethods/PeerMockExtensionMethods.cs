using System.Threading.Tasks;
using Moq;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Test.ExtensionMethods
{
    public static class PeerMockExtensionMethods
    {
        public static Mock<IPeer> SetupFakeHandshake(this Mock<IPeer> peerMock)
        {
            peerMock
                .Setup(x => x.Send(It.IsAny<VersionMessage>()))
                .Returns(Task.CompletedTask);

            var verAckMessage = new VerAckMessage();

            peerMock
                .Setup(x => x.Send(new VerAckMessage()))
                .Returns(Task.CompletedTask);

            peerMock
                .Setup(x => x.Receive<VerAckMessage>())
                .Returns(Task.FromResult(verAckMessage));

            return peerMock;
        }
    }
}
