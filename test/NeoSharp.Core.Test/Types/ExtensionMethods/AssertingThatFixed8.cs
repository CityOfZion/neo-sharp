using FluentAssertions;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Types.ExtensionMethods
{
    public static class AssertingThatFixed8
    {
        public static AssertingThat<Fixed8> IsZero(this AssertingThat<Fixed8> that)
        {
            that.Assertable.Should().Be(Fixed8.Zero);
            return that;
        }

        public static AssertingThat<Fixed8> AsLongValue(this AssertingThat<Fixed8> that, long longValue)
        {
            that.Assertable.Value.Should().Be(longValue);
            return that;
        }

        public static AssertingThat<Fixed8> IsEqual(this AssertingThat<Fixed8> that, object secondValue)
        {
            that.Assertable.Equals(secondValue).Should().BeTrue();
            return that;
        }

        public static AssertingThat<Fixed8> IsNotEqual(this AssertingThat<Fixed8> that, object secondValue)
        {
            that.Assertable.Equals(secondValue).Should().BeFalse();
            return that;
        }

        public static AssertingThat<Fixed8> IsGreaterThan(this AssertingThat<Fixed8> that, Fixed8 secondValue)
        {
            (that.Assertable > secondValue).Should().BeTrue();
            return that;
        }

        public static AssertingThat<Fixed8> IsSmallerThan(this AssertingThat<Fixed8> that, Fixed8 secondValue)
        {
            (that.Assertable < secondValue).Should().BeTrue();
            return that;
        }
    }
}
