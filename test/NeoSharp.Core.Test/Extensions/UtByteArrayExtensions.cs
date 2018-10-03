using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtByteArrayExtensions
    {
        // Arrange
        byte[] _data = new byte[] { 157, 179, 60, 8, 66, 122, 255, 105, 126, 49, 180, 74, 212, 41, 126, 177, 14, 255, 59, 82, 218, 113, 248, 145, 98, 5, 128, 140, 42, 70, 32, 69 };

        [TestMethod]
        public void Sha256()
        {
            // Arrange
            var hash1 = "0x2e58bad76961a22f12fe3508b1f6eb48c8d1724c556683c5a8f5b536eefbc6dc".HexToBytes();
            var hash2 = "0x2a3e06817fd1ea30f6a68b8647777135cd8e39e81382bbb67345d74550c6a8bd".HexToBytes();

            // Act
            var value1 = _data.Sha256();
            var value2 = _data.Sha256(3, 10);

            // Assert
            CollectionAssert.AreEqual(hash1, value1);
            CollectionAssert.AreEqual(hash2, value2);
        }

        [TestMethod]
        public void Hash256()
        {
            // Arrange
            var hash1 = "0x8fa20ade934ed14293f0155cd87023eb4413255867a06ffacc1e76128e5276f0".HexToBytes();
            var hash2 = "0x59f0f29b026ef005b12bdc066f6e6f4891d2b1d3534db1aee644453d96bfcfed".HexToBytes();

            // Act
            var value1 = _data.Hash256();
            var value2 = _data.Hash256(1, 4);

            // Assert
            CollectionAssert.AreEqual(hash1, value1);
            CollectionAssert.AreEqual(hash2, value2);
        }

        [TestMethod]
        public void ToScriptHash()
        {
            // Arrange
            var hash = "0xeb91fdd0868c68ac8dabb8c7e1abdb8325a26a97";

            // Act
            var value = _data.ToScriptHash();

            // Assert
            Assert.AreEqual(hash, value.ToString());
        }

        [TestMethod]
        public void XOR()
        {
            // Arrange
            var data = new byte[] { 15, 07, 17, 09, 60, 08, 06, 06, 12, 02, 25, 05, 10, 05, 12, 06, 49, 01, 80, 07, 04, 21, 02, 41, 01, 26, 01, 77, 01, 04, 25, 05 };
            var result = new byte[] { 146, 180, 45, 1, 126, 114, 249, 111, 114, 51, 173, 79, 222, 44, 114, 183, 63, 254, 107, 85, 222, 100, 250, 184, 99, 31, 129, 193, 43, 66, 57, 64 };

            // Act
            var value = _data.XOR(data);

            // Assert
            CollectionAssert.AreEqual(result, value);
        }

        [TestMethod]
        public void ToHexString()
        {
            // Arrange
            var hash = "0x9db33c08427aff697e31b44ad4297eb10eff3b52da71f8916205808c2a462045";

            // Act
            var value = _data.ToHexString(true);

            // Assert
            Assert.AreEqual(hash, value);
        }
    }
}
