using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.DI;
using NeoSharp.Core.DI;
using NeoSharp.Core.DI.Modules;
using NeoSharp.Core.Network;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtNetworkModule : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var containerBuilderMock = AutoMockContainer.GetMock<IContainerBuilder>();
            var module = AutoMockContainer.Create<NetworkModule>();

            // Act
            module.Register(containerBuilderMock.Object);

            // Assert
            containerBuilderMock.Verify(x => x.RegisterSingleton<NetworkConfig>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<INetworkManager, NetworkManager>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IServer, Server>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IPeerFactory, PeerFactory>(), Times.Once);
        }
    }
}
