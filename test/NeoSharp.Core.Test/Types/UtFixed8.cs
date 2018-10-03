using System;
using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Test.Types.ExtensionMethods;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Types
{
    [TestClass]
    public class UtFixed8
    {
        [TestMethod]
        public void Ctor_EmptyParameters_ObjectCreatedIsZero()
        {
            var actual = new Fixed8();

            Asserting
                .That(actual)
                .IsZero();
        }

        [TestMethod]
        public void Ctor_WithLongMaxValue_ObjectCreateWithLongMaxValue()
        {
            const long expectedLongValue = long.MaxValue;

            var actual = new Fixed8(expectedLongValue);

            Asserting
                .That(actual)
                .AsLongValue(expectedLongValue);
        }

        [TestMethod]
        public void Equal_TwoFixed8FieldsWithSameValue_IsEqual()
        {
            const long expectedLongValue = long.MaxValue;

            var firstOperator = new Fixed8(expectedLongValue);
            var secondOperator = new Fixed8(expectedLongValue);

            Asserting
                .That(firstOperator)
                .IsEqual(secondOperator);
        }

        [TestMethod]
        public void Equal_TwoFixed8FieldWithDifferentValues_NotEqual()
        {
            const long expectedLongValue = long.MaxValue;

            var firstOperator = new Fixed8(expectedLongValue);
            var secondOperator = new Fixed8();

            Asserting
                .That(firstOperator)
                .IsNotEqual(secondOperator);
        }

        [TestMethod]
        public void Equal_Fixed8AndNull_NotEqual()
        {
            const long expectedLongValue = long.MaxValue;

            var firstOperator = new Fixed8(expectedLongValue);

            Asserting
                .That(firstOperator)
                .IsNotEqual(null);
        }

        [TestMethod]
        public void GreaterThan_OperatorOneIsGreaterOperatorTwo_ReturnTrue()
        {
            var operatorOne = new Fixed8(10);
            var operatorTwo = new Fixed8(5);

            Asserting
                .That(operatorOne)
                .IsGreaterThan(operatorTwo);
        }

        [TestMethod]
        public void SmallerThan_OperatorOneIsSmallerOperatorTwo_ReturnTrue()
        {
            var operatorOne = new Fixed8(5);
            var operatorTwo = new Fixed8(10);

            Asserting
                .That(operatorOne)
                .IsSmallerThan(operatorTwo);
        }

        [TestMethod]
        public void Parse_ValidLongValueAsString_ParseIsOk()
        {
            const long longValue = 1;
            const int expectedFixed8LongValue = 100000000;

            var actual = Fixed8.Parse(longValue.ToString());

            Asserting
                .That(actual)
                .AsLongValue(expectedFixed8LongValue);

        }

        [TestMethod]
        public void Parse_InvalidLongValueAsString_ParseReturnFormatException()
        {
            const string invalidString = "xxx";

            Action parseAction = () => Fixed8.Parse(invalidString);

            parseAction
                .Should()
                .Throw<FormatException>();
        }

        [TestMethod]
        public void TryParse_ValidLongValueAsString_ParseIsOk()
        {
            const long longValue = 1;
            const int expectedFixed8LongValue = 100000000;

            var tryParseResult = Fixed8.TryParse(longValue.ToString(), out var fixed8Result);

            tryParseResult.Should().BeTrue();
            Asserting
                .That(fixed8Result)
                .AsLongValue(expectedFixed8LongValue);
        }

        [TestMethod]
        public void TryParse_InvalidLongValueAsString_ParseReturnFormatException()
        {
            const string invalidString = "xxx";

            var tryParseResult = Fixed8.TryParse(invalidString, out var fixed8Result);

            tryParseResult.Should().BeFalse();
            Asserting
                .That(fixed8Result)
                .IsZero();                  // TODO #411 [AboimPinto]: Don't know if ZERO is the correct value when the TryParse fail.
        }

        [TestMethod]
        public void TryParse_MaxLongValueAsString_ParseReturnZero()
        {
            var maxLongString = long.MaxValue.ToString();

            var tryParseResult = Fixed8.TryParse(maxLongString, out var fixed8Result);

            tryParseResult.Should().BeFalse();
            Asserting
                .That(fixed8Result)
                .IsZero();                  // TODO #411 [AboimPinto]: Don't know if ZERO is the correct value when the TryParse fail.
        }

        [TestMethod]
        public void TryParse_MinLongValueAsString_ParseReturnZero()
        {
            var minLongString = long.MinValue.ToString();

            var tryParseResult = Fixed8.TryParse(minLongString, out var fixed8Result);

            tryParseResult.Should().BeFalse();
            Asserting
                .That(fixed8Result)
                .IsZero();                  // TODO #411 [AboimPinto]: Don't know if ZERO is the correct value when the TryParse fail.
        }

        [TestMethod]
        public void Max_ReturnFixed8ObjectWithLongMaxValue()
        {
            const long expectedLongValue = long.MaxValue;

            var maxFixed8 = Fixed8.MaxValue;

            Asserting
                .That(maxFixed8)
                .AsLongValue(expectedLongValue);
        }

        [TestMethod]
        public void Max_ProvideSeveralFixed8Objects_ReturnObjectWithMaxLongValue()
        {
            var zeroFixed8 = new Fixed8();
            var maxFixed8 = Fixed8.MaxValue;
            var minFixed8 = Fixed8.MinValue;

            var max = new[] {zeroFixed8, maxFixed8, minFixed8}.Max();

            Asserting
                .That(maxFixed8)
                .IsEqual(max);
        }

        [TestMethod]
        public void Min_ReturnFixed8ObjectWithLongMinValue()
        {
            const long expectedLongValue = long.MinValue;

            var minFixed8 = Fixed8.MinValue;

            Asserting
                .That(minFixed8)
                .AsLongValue(expectedLongValue);
        }

        [TestMethod]
        public void Min_ProvideSeveralFixed8Objects_ReturnObjectWithMinLongValue()
        {
            var zeroFixed8 = new Fixed8();
            var maxFixed8 = Fixed8.MaxValue;
            var minFixed8 = Fixed8.MinValue;

            var min = new[] { zeroFixed8, maxFixed8, minFixed8 }.Min();

            Asserting
                .That(minFixed8)
                .IsEqual(min);
        }

        [TestMethod]
        public void Abs_ProvidePositiveFixed8_ReturnTheSameValue()
        {
            var positiveFixed8 = new Fixed8(1);

            var resultAbsoluteFixed8 = positiveFixed8.Abs();

            Asserting
                .That(resultAbsoluteFixed8)
                .IsEqual(positiveFixed8);
        }

        [TestMethod]
        public void Abs_ProvideNegativeFixed8_ReturnPositiveVersionOfTheFixed8()
        {
            var negativeFixed8 = new Fixed8(-1);
            var expectedPositiveLongValue = negativeFixed8.Value * -1;

            var resultAbsoluteFixed8 = negativeFixed8.Abs();

            Asserting
                .That(resultAbsoluteFixed8)
                .AsLongValue(expectedPositiveLongValue);
        }

        [TestMethod]
        public void Ceilling_Provide100000000Fixed8Object_NoCellingNeeded()
        {
            var actual = new Fixed8(100000000);

            var cellingValue = actual.Ceiling();

            Asserting
                .That(cellingValue)
                .IsEqual(actual);
        }

        [TestMethod]
        public void Ceilling_Provide500000Fixed8Object_RemaiderGreaterThanZeroLogic()
        {
            var actual = new Fixed8(500000);
            var expectedCeillingFixed8 = new Fixed8(100000000);

            var cellingValue = actual.Ceiling();

            Asserting
                .That(cellingValue)
                .IsEqual(expectedCeillingFixed8);
        }

        [TestMethod]
        public void Ceilling_Provide100500000Fixed8Object_RemaiderLessThanZeroLogic()
        {
            var actual = new Fixed8(-100500000);
            var expectedCeillingFixed8 = new Fixed8(-100000000);

            var cellingValue = actual.Ceiling();

            Asserting
                .That(cellingValue)
                .IsEqual(expectedCeillingFixed8);
        }

        [TestMethod]
        public void GetHashCode_ReturnRightHashCodeForTheLongValue()
        {
            var expectedHashCode = 0.GetHashCode();

            var actual = new Fixed8();

            actual.GetHashCode().Should().Be(expectedHashCode);
        }

        [TestMethod]
        public void FromDecimal_ValidValue_ReturnValidFixed8Object()
        {
            const decimal validDecimalValue = 1000;

            var actual = Fixed8.FromDecimal(validDecimalValue);

            actual.Should().BeOfType(typeof(Fixed8));
            actual.ToString().Should().Be(validDecimalValue.ToString(CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void FromDecimal_ProvideMaxLong_ThrowOverflowException()
        {
            const decimal invalidDecimalValue = long.MaxValue;

            Action fromDecimalAction = () =>
            {
                Fixed8.FromDecimal(invalidDecimalValue);
            };

            fromDecimalAction.Should().Throw<OverflowException>();
        }

        [TestMethod]
        public void FromDecimal_ProvideMinLong_ThrowOverflowException()
        {
            const decimal invalidDecimalValue = long.MinValue;

            Action fromDecimalAction = () =>
            {
                Fixed8.FromDecimal(invalidDecimalValue);
            };

            fromDecimalAction.Should().Throw<OverflowException>();
        }

        [TestMethod]
        public void ToString_ProvideValidZeroLongValue_ReturnSameLongValueAsString()
        {
            var expectedLongValueString = "0";

            var actual = new Fixed8();

            actual.ToString().Should().Be(expectedLongValueString);
        }

        [TestMethod]
        public void ToString_ProvideValidLongValue_ReturnSameLongValueAsString()
        {
            var expectedLongValueString = "0.00001";

            var actual = new Fixed8(1000);

            actual.ToString().Should().Be(expectedLongValueString);
        }

        [TestMethod]
        public void ToString_ProvideValidLongValueAndFormat_ReturnSameLongValueAsStringInTheRightFormat()
        {
            const string expectedLongValueString = "1000";

            var actual = new Fixed8(100000000000);

            actual.ToString("#.##").Should().Be(expectedLongValueString);
        }

        [TestMethod]
        public void ToString_ProvideValidLongValueAndFormatAndInvariantCulture_ReturnSameLongValueAsStringInTheRightFormatUsingTheCulture()
        {
            const string expectedLongValueString = "1000";

            var actual = new Fixed8(100000000000);

            actual.ToString("#.##", CultureInfo.InvariantCulture).Should().Be(expectedLongValueString);
        }

        [TestMethod]
        public void OperatorEqual_EqualElements_ReturnTrue()
        {
            var operatorOne = new Fixed8();
            var operatorTwo = new Fixed8();

            (operatorOne == operatorTwo).Should().BeTrue();
        }

        [TestMethod]
        public void OperatorEqual_DifferentElements_ReturnFalse()
        {
            var operatorOne = new Fixed8();
            var operatorTwo = new Fixed8(1);

            (operatorOne == operatorTwo).Should().BeFalse();
        }

        [TestMethod]
        public void OperatorGreaterOrEqualThan_OperatorOneGreaterThanOperatorTwo_ReturnTrue()
        {
            var operatorOne = new Fixed8(10);
            var operatorTwo = new Fixed8(5);

            (operatorOne >= operatorTwo).Should().BeTrue();
        }

        [TestMethod]
        public void OperatorGreaterOrEqualThan_OperatorOneEqualOperatorTwo_ReturnTrue()
        {
            var operatorOne = new Fixed8(10);
            var operatorTwo = new Fixed8(10);

            (operatorOne >= operatorTwo).Should().BeTrue();
        }

        [TestMethod]
        public void OperatorGreaterOrEqualThan_OperatorOneSmallerThanOperatorTwo_ReturnFalse()
        {
            var operatorOne = new Fixed8(5);
            var operatorTwo = new Fixed8(10);

            (operatorOne >= operatorTwo).Should().BeFalse();
        }

        [TestMethod]
        public void OperatorLessOrEqualThan_OperatorOneLessThanOperatorTwo_ReturnTrue()
        {
            var operatorOne = new Fixed8(5);
            var operatorTwo = new Fixed8(10);

            (operatorOne <= operatorTwo).Should().BeTrue();
        }

        [TestMethod]
        public void OperatorLessOrEqualThan_OperatorOneEqualOperatorTwo_ReturnTrue()
        {
            var operatorOne = new Fixed8(10);
            var operatorTwo = new Fixed8(10);

            (operatorOne <= operatorTwo).Should().BeTrue();
        }

        [TestMethod]
        public void OperatorLessOrEqualThan_OperatorOneGreaterThanOperatorTwo_ReturnFalse()
        {
            var operatorOne = new Fixed8(10);
            var operatorTwo = new Fixed8(5);

            (operatorOne <= operatorTwo).Should().BeFalse();
        }

        [TestMethod]
        public void OperatorEnequality_DifferentOperators_ReturnTrue()
        {
            var operatorOne = new Fixed8(10);
            var operatorTwo = new Fixed8(5);

            (operatorOne != operatorTwo).Should().BeTrue();
        }

        [TestMethod]
        public void OperatorEnequality_EqualOperators_ReturnFalse()
        {
            var operatorOne = new Fixed8(10);
            var operatorTwo = new Fixed8(10);

            (operatorOne != operatorTwo).Should().BeFalse();
        }

        [TestMethod]
        public void OperatorSubtraction_ReturnDifferenceBetweenValues()
        {
            var operatorOne = new Fixed8(10);
            var operatorTwo = new Fixed8(5);

            var actual = operatorOne - operatorTwo;

            Asserting
                .That(actual)
                .AsLongValue(5);
        }

        [TestMethod]
        public void OperatorAddition_ReturnSumOfValues()
        {
            var operatorOne = new Fixed8(10);
            var operatorTwo = new Fixed8(5);

            var actual = operatorOne + operatorTwo;

            Asserting
                .That(actual)
                .AsLongValue(15);
        }

        [TestMethod]
        public void OperatorMultiply_TwoFixed8Values_ReturnMultiplicationOfValues()
        {
            var operatorOne = new Fixed8(500000000);
            var operatorTwo = new Fixed8(500000000);

            var actual = operatorOne * operatorTwo;

            Asserting
                .That(actual)
                .AsLongValue(2500000000);
        }

        [TestMethod]
        public void OperatorMultiply_Fixed8ValueAndLongValue_ReturnMultiplicationOfValues()
        {
            var operatorOne = new Fixed8(500000000);
            const long operatorTwo = 5;

            var actual = operatorOne * operatorTwo;

            Asserting
                .That(actual)
                .AsLongValue(2500000000);
        }

        [TestMethod]
        public void OperatorUnaryNagation_ReturnNegativeFixed8()
        {
            var operatorOne = new Fixed8(1);

            var actual = -operatorOne;

            Asserting
                .That(actual)
                .AsLongValue(-1);
        }

        [TestMethod]
        public void OperatorDivision_TwoFixed8Values_ReturnDivisionOfValues()
        {
            var operatorOne = new Fixed8(500000000);
            const long operatorTwo = 5;

            var actual = operatorOne / operatorTwo;

            Asserting
                .That(actual)
                .AsLongValue(100000000);
        }

        [TestMethod]
        public void OperatorExplicit_Fixed8Value_ReturnLongValue()
        {
            var operatorOne = new Fixed8(100000000);

            var actual = (long)operatorOne;

            actual.Should().Be(1);
        }
    }
}