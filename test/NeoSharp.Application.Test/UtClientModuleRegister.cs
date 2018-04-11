using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.Client;
using NeoSharp.Application.DI;
using NeoSharp.Core;
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
            var simpleInjectorWrapperMock = this.AutoMockContainer.GetMock<ISimpleInjectorWrapper>();
            var module = this.AutoMockContainer.Create<ClientModuleRegister>();

            // Act
            module.Register(simpleInjectorWrapperMock.Object);

            // Assert
            simpleInjectorWrapperMock.Verify(x => x.Register<IBootstrapper, ClientManager>(), Times.Once);
            simpleInjectorWrapperMock.Verify(x => x.RegisterSingleton<IPrompt, Prompt>(), Times.Once);
            simpleInjectorWrapperMock.Verify(x => x.RegisterSingleton<IConsoleReader, ConsoleReader>(), Times.Once);
            simpleInjectorWrapperMock.Verify(x => x.RegisterSingleton<IConsoleWriter, ConsoleWriter>(), Times.Once);
        }
    }
}
