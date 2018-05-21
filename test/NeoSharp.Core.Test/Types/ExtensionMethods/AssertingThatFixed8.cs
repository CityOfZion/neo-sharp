using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Types.ExtensionMethods
{
    public static class AssertingThatFixed8
    {
        public static AssertingThat<Fixed8> IsZero(this AssertingThat<Fixed8> that)
        {
            Assert.AreEqual(Fixed8.Zero, that.Assertable);
            return that;
        }

        public static AssertingThat<Fixed8> AsLongValue(this AssertingThat<Fixed8> that, long longValue)
        {
            Assert.AreEqual(longValue, that.Assertable.GetData());
            return that;
        }

        public static AssertingThat<Fixed8> IsEqual(this AssertingThat<Fixed8> that, object secondValue)
        {
            Assert.IsTrue(that.Assertable.Equals(secondValue));
            return that;
        }

        public static AssertingThat<Fixed8> IsNotEqual(this AssertingThat<Fixed8> that, object secondValue)
        {
            Assert.IsFalse(that.Assertable.Equals(secondValue));
            return that;
        }

        public static AssertingThat<Fixed8> IsGreaterThan(this AssertingThat<Fixed8> that, Fixed8 secondValue)
        {
            Assert.IsTrue(that.Assertable > secondValue);
            return that;
        }

        public static AssertingThat<Fixed8> IsSmallerThan(this AssertingThat<Fixed8> that, Fixed8 secondValue)
        {
            Assert.IsTrue(that.Assertable < secondValue);
            return that;
        }
    }
}
