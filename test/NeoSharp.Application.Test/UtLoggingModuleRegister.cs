using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.DI;
using NeoSharp.Core.Logging;
using NeoSharp.Logging.NLog;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtLoggingModuleRegister : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var simpleInjectorWrapperMock = this.AutoMockContainer.GetMock<ISimpleInjectorWrapper>();
            var module = this.AutoMockContainer.Create<LoggingModuleRegister>();

            // Act
            module.Register(simpleInjectorWrapperMock.Object);

            // Assert
            simpleInjectorWrapperMock.Verify(x => x.RegisterSingleton<ILoggerFactory, NLogLoggerFactory>(), Times.Once);
            simpleInjectorWrapperMock.Verify(x => x.RegisterSingleton(typeof(ILogger<>), typeof(LoggerAdapter<>)), Times.Once);
        }
    }
}
