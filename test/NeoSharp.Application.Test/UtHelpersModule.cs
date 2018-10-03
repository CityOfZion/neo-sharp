using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.DI;
using NeoSharp.Core.DI.Modules;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Wallet.Wrappers;
using NeoSharp.Cryptography;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UtHelpersModule : TestBase
    {
        [TestMethod]
        public void Register_AllObjectsAreCorrectlyRegister()
        {
            // Arrange
            var containerBuilderMock = AutoMockContainer.GetMock<IContainerBuilder>();
            var module = AutoMockContainer.Create<HelpersModule>();

            // Act
            module.Register(containerBuilderMock.Object);

            // Assert
            containerBuilderMock.Verify(x => x.RegisterSingleton<IAsyncDelayer, AsyncDelayer>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<Crypto, BouncyCastleCrypto>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IFileWrapper, FileWrapper>(), Times.Once);
            containerBuilderMock.Verify(x => x.RegisterSingleton<IJsonConverter, JsonConverterWrapper>(), Times.Once);
        }
    }
}