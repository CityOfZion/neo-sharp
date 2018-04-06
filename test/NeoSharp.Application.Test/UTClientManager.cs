using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Application.Client;
using NeoSharp.TestHelpers;
using FluentAssertions;

namespace NeoSharp.Application.Test
{
    [TestClass]
    public class UTClientManager : TestBase
    {
        [TestMethod]
        public void Ctor_ConstructValidObject()
        {
            // Arrange 
            // Act
            var clientManager = this.AutoMockContainer.Create<ClientManager>();

            // Assert            
            clientManager.Should().BeOfType<ClientManager>();
        }

        [TestMethod]
        public void RunClient_NullArguments_NullArgumentIsUsedInStartPrompt()
        {
            // Arrange
            string[] expectedParameters = null;

            var promptMock = this.AutoMockContainer.GetMock<IPrompt>();
                
            var clientManager = this.AutoMockContainer.Create<ClientManager>();

            //Act
            clientManager.RunClient(expectedParameters);

            // Asset
            promptMock.Verify(x => x.StartPrompt(expectedParameters), Times.Once);
        }

        [TestMethod]
        public void RunClient_OneArgument_OneArgumentIsUsedInStartPrompt()
        {
            // Arrange
            var expectedParameters = new string[] { "CoZ" };

            var promptMock = this.AutoMockContainer.GetMock<IPrompt>();

            var clientManager = this.AutoMockContainer.Create<ClientManager>();

            //Act
            clientManager.RunClient(expectedParameters);

            // Asset
            promptMock.Verify(x => x.StartPrompt(expectedParameters), Times.Once);
        }

        [TestMethod]
        public void RunClient_MultipleArguments_MultipleArgumentsAreUsedInStartPrompt()
        {
            // Arrange
            var expectedParameters = new string[] { "City", "of", "Zion" };

            var promptMock = this.AutoMockContainer.GetMock<IPrompt>();

            var clientManager = this.AutoMockContainer.Create<ClientManager>();

            //Act
            clientManager.RunClient(expectedParameters);

            // Asset
            promptMock.Verify(x => x.StartPrompt(expectedParameters), Times.Once);
        }
    }
}
