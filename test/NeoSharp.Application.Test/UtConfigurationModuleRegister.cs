using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.DI;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtConfigurationModuleRegister : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var simpleInjectorWrapperMock = this.AutoMockContainer.GetMock<ISimpleInjectorWrapper>();
            var module = this.AutoMockContainer.Create<ConfigurationModuleRegister>();

            // Act
            module.Register(simpleInjectorWrapperMock.Object);

            // Assert
            simpleInjectorWrapperMock.Verify(
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

