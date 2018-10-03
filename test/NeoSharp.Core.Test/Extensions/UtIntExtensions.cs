using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtIntExtensions
    {
        // Arrange
        readonly byte[] _data = new byte[] { 157, 179, 60, 8, 66, 122, 255, 105, 126, 49, 180, 74, 212, 41, 126, 177, 14, 255, 59, 82, 218, 113, 248, 145, 98, 5, 128, 140, 42, 70, 32, 69 };
        
        [TestMethod]
        public void ToInt32()
        {
            // Act
            var test = _data.ToInt32();

            // Assert
            Assert.AreEqual(138195869, test);
        }

        [TestMethod]
        public void ToInt64()
        {
            // Act
            var test = _data.ToInt64();

            // Assert
            Assert.AreEqual(7637957917068276637, test);
        }

        [TestMethod]
        public void ToUInt16()
        {
            // Act
            var test = _data.ToUInt16();

            // Assert
            Assert.AreEqual(test, 45981);
        }

        [TestMethod]
        public void ToUInt32()
        {
            // Act
            var test = _data.ToUInt32();

            // Assert
            Assert.AreEqual(138195869U, test);
        }

        [TestMethod]
        public void ToUInt64()
        {
            // Act
            var test = _data.ToUInt64();

            // Assert
            Assert.AreEqual(7637957917068276637U, test);
        }

        [TestMethod]
        public void ToInt32_with_index()
        {
            // Act
            var test = _data.ToInt32(3);

            // Assert
            Assert.AreEqual(test, -8764920);
        }

        [TestMethod]
        public void ToInt64_with_index()
        {
            // Act
            var test = _data.ToInt64(3);

            // Assert
            Assert.AreEqual(-5462445879300832760, test);
        }

        [TestMethod]
        public void ToUInt16_with_index()
        {
            // Act
            var test = _data.ToUInt16(3);

            // Assert
            Assert.AreEqual(16904, test);
        }

        [TestMethod]
        public void ToUInt32_with_index()
        {
            // Act
            var test = _data.ToUInt32(3);

            // Assert
            Assert.AreEqual(4286202376U, test);
        }

        [TestMethod]
        public void ToUInt64_with_index()
        {
            // Act
            var test = _data.ToUInt64(3);

            // Assert
            Assert.AreEqual(12984298194408718856U, test);
        }
    }
}