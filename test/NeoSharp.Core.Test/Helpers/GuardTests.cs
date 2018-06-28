using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Helpers;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Helpers
{
    [TestClass]
    public class GuardTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowIfNull_ArgumentWithNullValue_ArgumentNullExceptionThrow()
        {
            Guard.ThrowIfNull<string>(null, "argumentName");
        }

        [TestMethod]
        public void ThrowIfNull_ArgumentWithWithValidValue_ReturnTheArgument()
        {
            const string expectedArgumentValue = "argumentValue";

            var result = Guard.ThrowIfNull<string>(expectedArgumentValue, "argumentName");

            Assert.AreEqual(expectedArgumentValue, result);
        }
    }
}
