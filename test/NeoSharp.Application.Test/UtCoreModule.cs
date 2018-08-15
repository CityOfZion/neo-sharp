using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.DI;
using NeoSharp.Core.DI.Modules;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtCoreModule : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var containerBuilderMock = AutoMockContainer.GetMock<IContainerBuilder>();
            var module = AutoMockContainer.Create<CoreModule>();

            // Act
            module.Register(containerBuilderMock.Object);

            // Assert
            containerBuilderMock.Verify(x => x.RegisterModule<BlockchainModule>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterModule<NetworkModule>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterModule<HelpersModule>(), Times.Once);
        }
    }
}