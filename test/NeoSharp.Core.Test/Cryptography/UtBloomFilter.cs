using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Cryptography
{
    [TestClass]
    public class UtBloomFilter : TestBase
    {
        [TestMethod]
        public void Add()
        {
            // Arrange
            var bloomfilter = new BloomFilter(256, 2, 1);
            byte[] element = { 0x01, 0x01, 0x01, 0x01 };
            var result = new byte[32];

            // Act
            bloomfilter.Add(element);

            // Assert
            bloomfilter.GetBits(result);
            Assert.AreEqual(0x08, result[8]);
        }

        [TestMethod]
        public void Check()
        {
            // Arrange
            var bloomfilter = new BloomFilter(256, 2, 1);
            byte[] element1 = { 0x01, 0x02, 0x03, 0x04 };
            byte[] element2 = { 0xDE, 0xAD, 0xBE, 0xEF };

            // Act
            bloomfilter.Add(element1);
            var result = bloomfilter.Check(element2);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetBits()
        {
            // Arrange
            var bloomfilter = new BloomFilter(32, 3, 2);
            byte[] element1 = { 0x01, 0x02, 0x03, 0x04 };
            byte[] element2 = { 0xDE, 0xAD, 0xBE, 0xEF };
            byte[] element3 = { 0x22, 0x22, 0x22, 0x22 };
            byte[] expect = { 0x0a, 0x04, 0x44, 0xca };
            var result = new byte[4];

            // Act
            bloomfilter.Add(element1);
            bloomfilter.Add(element2);
            bloomfilter.Add(element3);

            // Assert
            bloomfilter.GetBits(result);
            Assert.AreEqual(result.ToHexString(false), "0a0444ca");
        }
    }
}
