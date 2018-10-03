using System.Linq;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Cryptography;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Test.Cryptography
{
    [TestClass]
    public class UtHelper
    {
        // Arrange
        private readonly string _password = "NEO rocks";
        private readonly byte[] _aeskey = "0xec75b41f88075c835725cadaeaeec153ce02c9cba1ebd63ae66b6252a564df4b".HexToBytes();

        [TestMethod]
        public void Password_to_AES_key()
        {
            // Act
            var result = Helper.ToAesKey(_password);

            // Assert
            CollectionAssert.AreEqual(_aeskey, result);
        }

        [TestMethod]
        public void Secure_Password_to_AES_key()
        {
            // Arrange
            var securepassword = new SecureString();
            _password.ToCharArray().ToList().ForEach(p => securepassword.AppendChar(p));

            // Act
            var result = Helper.ToAesKey(securepassword);

            // Assert
            CollectionAssert.AreEqual(_aeskey, result);
        }
    }
}