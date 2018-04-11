using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.DI;
using NeoSharp.Core.DI;
using NeoSharp.Core.Logging;
using NeoSharp.Logging.NLog;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtLoggingModule : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var containerBuilderMock = this.AutoMockContainer.GetMock<IContainerBuilder>();
            var module = this.AutoMockContainer.Create<LoggingModule>();

            // Act
            module.Register(containerBuilderMock.Object);

            // Assert
            containerBuilderMock.Verify(x => x.RegisterSingleton<ILoggerFactory, NLogLoggerFactory>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton(typeof(ILogger<>), typeof(LoggerAdapter<>)), Times.Once);
        }
    }
}
