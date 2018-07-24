using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtEnumerableExtensions
    {
        [TestMethod]
        public void Query_Contains()
        {
            // Arrange
            var test = new int[] { 1, 2, 3, 4, 5 };

            // Act
            var result = test.QueryResult("3", EnumerableExtensions.QueryMode.Contains).ToArray();

            // Assert
            CollectionAssert.AreEqual(new int[] { 3 }, result);
        }

        [TestMethod]
        public void Query_Regex()
        {
            // Arrange
            var test = new int[] { 1, 2, 3, 4, 5 };

            // Act
            var result = test.QueryResult("[0-9]", EnumerableExtensions.QueryMode.Regex).ToArray();

            // Assert
            CollectionAssert.AreEqual(new int[] { 1, 2, 3, 4, 5 }, result);
        }

        [TestMethod]
        public void Querys_Regex()
        {
            // Arrange
            var test = new int[] { 1, 2, 3, 4, 5 };
            var result = 0;

            // Act
            test.ForEach(a => result += a);

            // Assert
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void Query_Distinct()
        {
            // Arrange
            var test = new int[] { 1, 1, 2, 4, 4 };
            var result = new int[] { 1, 2, 4 };

            // Act
            var distinctValues = test.Distinct(a => a);

            // Assert
            CollectionAssert.AreEqual(result, distinctValues.ToArray());
        }
    }
}