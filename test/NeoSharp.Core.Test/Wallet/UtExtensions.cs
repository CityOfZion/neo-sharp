using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Wallet.Helpers;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Wallet.Test
{
    [TestClass]
    public class UtExtensions : TestBase
    {

        [TestMethod]
        public void TestIsValidPrivateKeyTooShort()
        {
            var p = new byte[] { 128, 33, 68, 4, 111, 111, 43, 161, 201, 123, 240, 182, 240, 48, 97, 165, 233, 223, 133, 20, 106, 12, 245, 240, 46, 69, 25, 1, 34, 115, 30, 37, 224 };
            var result = p.IsValidPrivateKey();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestIsValidPrivateKeyTooLong()
        {
            var p = new byte[] { 128, 33, 68, 4, 111, 111, 43, 161, 201, 123, 240, 182, 240, 48, 97, 165, 233, 223, 133, 20, 106, 12, 245, 240, 46, 69, 25, 1, 34, 115, 30, 37, 224, 1, 1 };
            var result = p.IsValidPrivateKey();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestIsValidPrivateKeyNotStartingWith128()
        {
            var p = new byte[] { 127, 33, 68, 4, 111, 111, 43, 161, 201, 123, 240, 182, 240, 48, 97, 165, 233, 223, 133, 20, 106, 12, 245, 240, 46, 69, 25, 1, 34, 115, 30, 37, 224, 1 };
            var result = p.IsValidPrivateKey();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestIsValidPrivateKeyNotEndingWith1()
        {
            var p = new byte[] { 128, 33, 68, 4, 111, 111, 43, 161, 201, 123, 240, 182, 240, 48, 97, 165, 233, 223, 133, 20, 106, 12, 245, 240, 46, 69, 25, 1, 34, 115, 30, 37, 224, 2 };
            var result = p.IsValidPrivateKey();
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestIsValidPrivateKey()
        {
            var p = new byte[] { 128, 33, 68, 4, 111, 111, 43, 161, 201, 123, 240, 182, 240, 48, 97, 165, 233, 223, 133, 20, 106, 12, 245, 240, 46, 69, 25, 1, 34, 115, 30, 37, 224, 1 };
            var result = p.IsValidPrivateKey();
            Assert.IsTrue(result);
        }

    }
}
