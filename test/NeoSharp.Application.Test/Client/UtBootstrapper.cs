using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Application.Client;
using NeoSharp.Core;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test.Client
{
    [TestClass]
    public class UtBootstrapper : TestBase
    {
        [TestMethod]
        public void Ctor_CreateValidInstanceOfBootstrapperAndImplementCorrectInterface()
        {
            var testee = this.AutoMockContainer.Create<Bootstrapper>();

            testee
                .Should()
                .BeOfType<Bootstrapper>()
                .And
                .BeAssignableTo<IBootstrapper>();
        }

        [TestMethod]
        public void Start_EmptyListOfArguments_NoArgumentsPassedToStartPrompt()
        {
            var emptyArgumentList = new string[] { };

            var promptMock = this.AutoMockContainer.GetMock<IPrompt>();

            var testee = this.AutoMockContainer.Create<Bootstrapper>();

            testee.Start(emptyArgumentList);

            promptMock.Verify(x => x.StartPrompt(emptyArgumentList));
        }

        [TestMethod]
        public void Start_ValidListOfArguments_ArgumentsArePassedToStartPrompt()
        {
            var argumentList = new string[] { "arg1", "arg2", "arg3" };

            var promptMock = this.AutoMockContainer.GetMock<IPrompt>();

            var testee = this.AutoMockContainer.Create<Bootstrapper>();

            testee.Start(argumentList);

            promptMock.Verify(x => x.StartPrompt(argumentList));
        }
    }
}
