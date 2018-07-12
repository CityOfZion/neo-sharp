using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtTypeExtensions
    {
        // Arrange
        class dummyChild<T> : idummy<T> { }
        interface idummy<T> { }

        [TestMethod]
        public void Type_is_generic_assignable()
        {
            // Act
            var type = typeof(idummy<>);
            var good = typeof(dummyChild<int>);
            var bad = typeof(int);

            // Assert
            Assert.IsTrue(good.IsAssignableToGenericType(type));
        }

        [TestMethod]
        public void Type_isnot_generic_assignable()
        {
            // Act
            var type = typeof(idummy<>);
            var good = typeof(dummyChild<int>);
            var bad = typeof(int);

            // Assert
            Assert.IsFalse(bad.IsAssignableToGenericType(type));
        }
    }
}