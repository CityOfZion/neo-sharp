using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtFixed8Extensions
    {
        class Test
        {
            public Fixed8 dummy;
        }
        Fixed8[] _test = new Fixed8[]  {
                new Fixed8(656565656),
                new Fixed8(987654321),
                new Fixed8(123456789)
                };

        [TestMethod]
        public void Sum_selector()
        {
            // Arrange
            Test[] test = {
            new Test() { dummy = new Fixed8(656565656) },
            new Test() { dummy = new Fixed8(987654321) },
            new Test() { dummy = new Fixed8(123456789) }
            };

            // Act
            var result = test.Sum(u => u.dummy);

            // Assert
            Assert.AreEqual(new Fixed8(1767676766), result);
        }

        [TestMethod]
        public void Sum()
        {
            // Act
            var result = _test.Sum();

            // Assert
            Assert.AreEqual(new Fixed8(1767676766), result);
        }

        [TestMethod]
        public void Max()
        {
            // Act
            var result = _test.Max();

            // Assert
            Assert.AreEqual(new Fixed8(987654321), result);
        }

        [TestMethod]
        public void Min()
        {
            // Act
            var result = _test.Min();

            // Assert
            Assert.AreEqual(new Fixed8(123456789), result);
        }
    }
}