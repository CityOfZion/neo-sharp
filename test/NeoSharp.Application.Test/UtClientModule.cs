using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.Client;
using NeoSharp.Application.DI;
using NeoSharp.Core;
using NeoSharp.Core.DI;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtClientModule : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var containerBuilderMock = AutoMockContainer.GetMock<IContainerBuilder>();
            var module = AutoMockContainer.Create<ClientModule>();

            // Act
            module.Register(containerBuilderMock.Object);

            // Assert
            containerBuilderMock.Verify(x => x.Register<IBootstrapper, Bootstrapper>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IPrompt, Prompt>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IConsoleHandler, ConsoleHandler>(), Times.Once);
        }
    }
}
