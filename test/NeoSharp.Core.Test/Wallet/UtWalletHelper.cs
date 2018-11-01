using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Wallet.Helpers;
using NeoSharp.Cryptography;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Wallet.Test
{
    [TestClass]
    public class UtWalletHelper : TestBase
    {

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestGetPrivateKeyFromWIFNull()
        {
            var testee = new WalletHelper();
            testee.GetPrivateKeyFromWIF(null);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void TestGetPrivateKeyFromWIFInvalidPrivateKey()
        {
            var invalidWif = Crypto.Default.Base58CheckEncode(new byte[] { 127, 33, 68, 4, 111, 111, 43, 161, 201, 123, 240, 182, 240, 48, 97, 165, 233, 223, 133, 20, 106, 12, 245, 240, 46, 69, 25, 1, 34, 115, 30, 37, 224, 1 });
            var testee = new WalletHelper();
            testee.GetPrivateKeyFromWIF(invalidWif);
        }

        [TestMethod]
        public void TestGetPrivateKeyFromWIF()
        {
            var wif = "KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a";
            var testee = new WalletHelper();
            var result = testee.GetPrivateKeyFromWIF(wif);
            var expected =  new byte[] { 33, 68, 4, 111, 111, 43, 161, 201, 123, 240, 182, 240, 48, 97, 165, 233, 223, 133, 20, 106, 12, 245, 240, 46, 69, 25, 1, 34, 115, 30, 37, 224 };
            CollectionAssert.AreEqual(expected, result);
        }
    }
}
