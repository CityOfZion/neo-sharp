using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Application.Client;
using NeoSharp.TestHelpers;

namespace NeoSharp.Application.Test.Client
{
    [TestClass]
    public class UtPromptUserVariables : TestBase
    {
        [TestMethod]
        public void TestAdd()
        {
            var variables = new PromptUserVariables();

            var name = RandomString(50);
            var value = RandomString(50);

            variables.Add(name, value);

            var changeMe = variables.Replace($"A${name}B");

            Assert.AreEqual(changeMe, $"A{value}B");
        }

        [TestMethod]
        public void TestRemove()
        {
            var variables = new PromptUserVariables();

            var name = RandomString(50);
            var value = RandomString(50);

            variables.Add(name, value);
            variables.Remove(name);

            var changeMe = variables.Replace($"A${name}B");

            Assert.AreEqual(changeMe, $"A${name}B");
        }
    }
}
