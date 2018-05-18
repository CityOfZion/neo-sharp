using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Cryptography;
using NeoSharp.TestHelpers;
using System;
using System.Text;

namespace NeoSharp.Core.Test.Cryptography
{
    [TestClass]
    public class UtBase58 : TestBase
    {
        [TestMethod]
        public void Base58_Encode()
        {
            // Arrange
            var test = "Base58 Unit Test Encode";

            // Act
            var result = Base58.Encode(Encoding.ASCII.GetBytes(test));

            // Asset
            Assert.AreEqual(result, "2NXDA7BcYgm9Sfmth7KzYhxPrNq8Gnj6");
        }

        [TestMethod]
        public void Base58_Decode()
        {
            // Arrange
            var test = "2NXDA7BcYgm9Sfmth7KzYhxPqrw4Qpuz";

            // Act
            var decoding = Base58.Decode(test);
            var result = Encoding.ASCII.GetString(decoding);

            // Asset
            Assert.AreEqual(result, "Base58 Unit Test Decode");
        }

        [TestMethod]
        public void Base58_Full()
        {
            // Arrange
            var test = RandomString(_rand.Next(1, 30));

            // Act
            var encodig = Base58.Encode(Encoding.ASCII.GetBytes(test));
            var decoding = Base58.Decode(encodig);
            var result = Encoding.ASCII.GetString(decoding);

            // Asset
            Assert.AreEqual(test, result);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Base58_Exception()
        {
            // Arrange
            var test = "QmFzZTU4IFVuaXQgVGVzdCBFeGNlcHRpb24=";

            // Act
            var decoding = Base58.Decode(test);
        }
    }
}
