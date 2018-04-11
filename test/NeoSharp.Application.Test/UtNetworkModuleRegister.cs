using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.DI;
using NeoSharp.Core.Network;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtNetworkModuleRegister : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var simpleInjectorWrapperMock = this.AutoMockContainer.GetMock<ISimpleInjectorWrapper>();
            var module = this.AutoMockContainer.Create<NetworkModuleRegister>();

            // Act
            module.Register(simpleInjectorWrapperMock.Object);

            // Assert
            simpleInjectorWrapperMock.Verify(x => x.RegisterSingleton<NetworkConfig>(), Times.Once);
            simpleInjectorWrapperMock.Verify(x => x.RegisterSingleton<INetworkManager, NetworkManager>(), Times.Once);
            simpleInjectorWrapperMock.Verify(x => x.RegisterSingleton<IServer, Server>(), Times.Once);
            simpleInjectorWrapperMock.Verify(x => x.RegisterTransientInstance<IPeer, Peer>(), Times.Once);
        }
    }
}
