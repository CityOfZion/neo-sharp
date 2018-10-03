using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using NeoSharp.Types;

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

            // Assert
            value.Should().NotBeNull();
            value.Value.Should().Be(0);
        }

        [TestMethod]
        public void Ctor_Parameters_ObjectCreateds()
        {
            // Arrange
            var value = new BigDecimal(1500, 3);

            // Assert
            value.Value.Should().Be(1500);
            value.Decimals.Should().Be(3);
            value.Sign.Should().Be(1);
        }

        [TestMethod]
        public void ChangeDecimals_Equal()
        {
            // Arrange
            var value = new BigDecimal(-5300, 3);

            // Act
            var newvalue = value.ChangeDecimals(3);

            // Assert
            newvalue.Value.Should().Be(value.Value);
            newvalue.Decimals.Should().Be(value.Decimals);
            newvalue.Decimals.Should().Be(value.Decimals);
            newvalue.Sign.Should().Be(value.Sign);
        }

        [TestMethod]
        public void ChangeDecimals_Bigger()
        {
            // Arrange
            var value = new BigDecimal(1500, 3);

            // Act
            var newvalue = value.ChangeDecimals(5);

            // Assert
            newvalue.Value.Should().Be(150000);
            newvalue.Decimals.Should().Be(5);
        }

        [TestMethod]
        public void ChangeDecimals_Lower()
        {
            // Arrange
            var value = new BigDecimal(150000, 5);

            // Act
            var newvalue = value.ChangeDecimals(3);

            // Assert
            newvalue.Value.Should().Be(1500);
            newvalue.Decimals.Should().Be(3);
        }

        [TestMethod]
        public void TryParse_String_with_E()
        {
            // Arrange
            var actual = BigDecimal.TryParse("1234e10", 3, out var value);

            // Assert
            actual.Should().BeTrue();
            value.Value.Should().Be(12340000000000000);
            value.Decimals.Should().Be(3);
        }

        [TestMethod]
        public void TryParse_String_with_point()
        {
            // Arrange
            var actual = BigDecimal.TryParse("1.234", 5, out var value);

            // Assert
            actual.Should().BeTrue();
            value.Value.Should().Be(123400);
            value.Decimals.Should().Be(5);
        }

        [TestMethod]
        public void Parse_String()
        {
            // Arrange
            var value = BigDecimal.Parse("1234", 5);

            // Assert
            value.Value.Should().Be(123400000);
            value.Decimals.Should().Be(5);
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

            // Assert
            f8value.Should().Be(newvalue);
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

            // Assert
            value.ToString().Should().Be("0.0000009876");
        }
    }
}