using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Test.Types
{
    [TestClass]
    public class UtBigDecimal
    {
        [TestMethod]
        public void Ctor_EmptyParameters_ObjectCreatedIsZero()
        {
            // Arrange
            var value = new BigDecimal();

            // Asset
            Assert.IsNotNull(value);
            Assert.AreEqual(0, value.Value);
        }

        [TestMethod]
        public void Ctor_Parameters_ObjectCreateds()
        {
            // Arrange
            var value = new BigDecimal(1500, 3);

            // Asset
            Assert.AreEqual(1500, value.Value);
            Assert.AreEqual(3, value.Decimals);
            Assert.AreEqual(1, value.Sign);
        }

        [TestMethod]
        public void ChangeDecimals_Equal()
        {
            // Arrange
            var value = new BigDecimal(-5300, 3);

            // Act
            var newvalue = value.ChangeDecimals(3);

            // Asset
            Assert.AreEqual(value.Value, newvalue.Value);
            Assert.AreEqual(value.Decimals, newvalue.Decimals);
            Assert.AreEqual(value.Sign, newvalue.Sign);
        }

        [TestMethod]
        public void ChangeDecimals_Bigger()
        {
            // Arrange
            var value = new BigDecimal(1500, 3);

            // Act
            var newvalue = value.ChangeDecimals(5);

            // Asset
            Assert.AreEqual(150000, newvalue.Value);
            Assert.AreEqual(5, newvalue.Decimals);
        }

        [TestMethod]
        public void ChangeDecimals_Lower()
        {
            // Arrange
            var value = new BigDecimal(150000, 5);

            // Act
            var newvalue = value.ChangeDecimals(3);

            // Asset
            Assert.AreEqual(1500, newvalue.Value);
            Assert.AreEqual(3, newvalue.Decimals);
        }

        [TestMethod]
        public void TryParse_String_with_E()
        {
            // Arrange
            var actual = BigDecimal.TryParse("1234e10", 3, out var value);

            // Asset
            Assert.IsTrue(actual);
            Assert.AreEqual(12340000000000000, value.Value);
            Assert.AreEqual(3, value.Decimals);
        }

        [TestMethod]
        public void TryParse_String_with_point()
        {
            // Arrange
            var actual = BigDecimal.TryParse("1.234", 5, out var value);

            // Asset
            Assert.IsTrue(actual);
            Assert.AreEqual(123400, value.Value);
            Assert.AreEqual(5, value.Decimals);
        }

        [TestMethod]
        public void Parse_String()
        {
            // Arrange
            var value = BigDecimal.Parse("1234", 5);

            // Asset
            Assert.AreEqual(123400000, value.Value);
            Assert.AreEqual(5, value.Decimals);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Parse_Exception()
        {
            // Arrange
            var value = BigDecimal.Parse("1,234", 5);
        }

        [TestMethod]
        public void BigDecimal_ToFixed8()
        {
            // Arrange
            var value = new BigDecimal(1500, 3);
            var f8value = new Fixed8(150000000);

            // Act
            var newvalue = value.ToFixed8();

            // Asset
            Assert.AreEqual(f8value, newvalue);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void ToFixed8_Exception()
        {
            // Arrange
            var value = new BigDecimal(ulong.MaxValue, 0);

            // Act
            var newvalue = value.ToFixed8();
        }

        [TestMethod]
        public void BigDecimal_ToString()
        {
            // Arrange
            var value = new BigDecimal(9876, 10);

            // Asset
            Assert.AreEqual("0.0000009876", value.ToString());
        }
    }
}