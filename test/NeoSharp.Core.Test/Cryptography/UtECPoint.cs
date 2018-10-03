using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Cryptography;
using NeoSharp.TestHelpers;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Test.Cryptography
{
    [TestClass]
    public class UtECPoint : TestBase
    {
        [TestMethod]
        public void Infinity_ECPoint()
        {
            // Arrange
            var value = ECPoint.Infinity;

            // Assert
            Assert.IsNotNull(value);
            Assert.AreEqual(BigInteger.Zero, value.X);
            Assert.AreEqual(BigInteger.Zero, value.Y);
            Assert.IsTrue(value.IsInfinity);
            CollectionAssert.AreEqual(new byte[] { 0x00 }, value.EncodedData);
            CollectionAssert.AreEqual(new byte[] { 0x00 }, value.DecodedData);
        }

        [TestMethod]
        public void Ctor_ECPoint()
        {
            // Arrange
            var pubkey_A = "0238356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6f".HexToBytes();
            var pubkey_B = "0438356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6fc88c3095b1b688d3d027477dfad0deb1ab94cb08db2de5abb79c1482aa1ea2fc".HexToBytes();

            // Act
            var value = new ECPoint(pubkey_A);

            // Assert
            Assert.AreEqual(value.X.ToString(), "25423910948081187308645163652542039167507182320027680753707589392465049496431");
            Assert.AreEqual(value.Y.ToString(), "90710263625294316789152749637780639904397777988680875310398951746483276129020");
            CollectionAssert.AreEqual(pubkey_A, value.EncodedData);
            CollectionAssert.AreEqual(pubkey_B, value.DecodedData);
            Assert.IsFalse(value.IsInfinity);
        }

        [TestMethod]
        public void Compare_Equal_Points()
        {
            // Arrange
            var pubkey_A = "0238356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6f".HexToBytes();
            var pubkey_B = "0438356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6fc88c3095b1b688d3d027477dfad0deb1ab94cb08db2de5abb79c1482aa1ea2fc".HexToBytes();
            var pointA = new ECPoint(pubkey_A);
            var pointB = new ECPoint(pubkey_B);

            // Act
            var result = pointA.CompareTo(pointB);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Compare_Different_Points()
        {
            // Arrange
            var pubkey_A = "0238356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6f".HexToBytes();
            var pubkey_B = "0324de2cc4fe4b20963a5bae8cdcd52f431cd08ab331197e70e1d66d94ff35dda2".HexToBytes();
            var point_A = new ECPoint(pubkey_A);
            var point_B = new ECPoint(pubkey_B);

            // Act
            var result_A = point_A.CompareTo(point_B);
            var result_B = point_B.CompareTo(point_A);

            // Assert
            Assert.AreEqual(1, result_A);
            Assert.AreEqual(-1, result_B);
        }

        [TestMethod]
        public void ECPoint_ToString()
        {
            // Arrange
            var pubkey_A = "0238356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6f".HexToBytes();
            var point_A = new ECPoint(pubkey_A);

            // Act
            var result = point_A.ToString();
            var result0x = point_A.ToString(true);

            // Assert
            Assert.AreEqual("0238356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6f", result);
            Assert.AreEqual("0x0238356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6f", result0x);
        }
    }
}
