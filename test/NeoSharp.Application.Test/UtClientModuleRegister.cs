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
    public class UtClientModuleRegister : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var containerBuilderMock = this.AutoMockContainer.GetMock<IContainerBuilder>();
            var module = this.AutoMockContainer.Create<ClientModule>();

            // Act
            module.Register(containerBuilderMock.Object);

            // Assert
            containerBuilderMock.Verify(x => x.Register<IBootstrapper, Bootstrapper>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IPrompt, Prompt>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IConsoleReader, ConsoleReader>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IConsoleWriter, ConsoleWriter>(), Times.Once);
        }
    }
}
