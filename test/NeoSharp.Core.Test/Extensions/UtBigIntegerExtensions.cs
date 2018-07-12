using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtBigIntegerExtensions
    {
        [TestMethod]
        public void Mod()
        {
            // Arrange
            var a = new BigInteger(-123456789);
            var b = new BigInteger(987654321);
            var result = new BigInteger(864197532);

            // Act
            var value = a.Mod(b);

            // Assert
            Assert.AreEqual(result, value);
        }

        [TestMethod]
        public void ModInverse()
        {
            // Arrange
            var a = new BigInteger(123456789);
            var b = new BigInteger(987654321);
            var result = new BigInteger(987654313);

            // Act
            var value = a.ModInverse(b);

            // Assert
            Assert.AreEqual(result, value);
        }

        [TestMethod]
        public void TestBit()
        {
            // Arrange
            var a = new BigInteger(987654321);

            // Act
            var value = a.TestBit(2);

            // Assert
            Assert.IsTrue(a.TestBit(4));
            Assert.IsFalse(a.TestBit(2));
        }
    }
}