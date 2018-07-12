using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtBitExtensions
    {
        [TestMethod]
        public void BitLen()
        {
            // Arrange
            var value1 = 0;
            var value2 = 123;
            var value3 = -123;

            //Assert
            Assert.AreEqual(0, value1.BitLen());
            Assert.AreEqual(7, value2.BitLen());
            Assert.AreEqual(32, value3.BitLen());
        }

        [TestMethod]
        public void GetBitLength()
        {
            // Arrange
            var value1 = new BigInteger();
            var value2 = new BigInteger(123);
            var value3 = new BigInteger(-123);

            // Assert
            Assert.AreEqual(8, value1.GetBitLength());
            Assert.AreEqual(7, value2.GetBitLength());
            Assert.AreEqual(7, value3.GetBitLength());
        }

        [TestMethod]
        public void GetLowestSetBit()
        {
            // Arrange
            var value1 = new BigInteger();
            var value2 = new BigInteger(123);
            var value3 = new BigInteger(4);

            // Assert
            Assert.AreEqual(-1, value1.GetLowestSetBit());
            Assert.AreEqual(0, value2.GetLowestSetBit());
            Assert.AreEqual(2, value3.GetLowestSetBit());
        }
    }
}
