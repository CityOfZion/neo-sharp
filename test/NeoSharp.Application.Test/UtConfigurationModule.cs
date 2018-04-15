using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.DI;
using NeoSharp.Core.DI;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtConfigurationModule : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var containerBuilderMock = AutoMockContainer.GetMock<IContainerBuilder>();
            var module = AutoMockContainer.Create<ConfigurationModule>();

            // Act
            module.Register(containerBuilderMock.Object);

            // Assert
            containerBuilderMock.Verify(
                x => x.Register<IConfiguration>(
                    It.Is<IConfigurationRoot>(c =>
                        c.Providers.Count() == 1 &&
                        ((JsonConfigurationProvider) c.Providers.Single()).Source.Path == "appsettings.json" &&
                        !((JsonConfigurationProvider)c.Providers.Single()).Source.Optional &&
                        ((JsonConfigurationProvider)c.Providers.Single()).Source.ReloadOnChange)),
                Times.Once);
        }
    }
}

