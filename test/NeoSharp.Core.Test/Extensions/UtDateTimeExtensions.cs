using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtDateTimeExtensions
    {
        [TestMethod]
        public void DateTime_to_uint()
        {
            // Arrange
            var date = new DateTime(2222, 4, 20, 6, 25, 21, DateTimeKind.Utc);

            // Act
            var value = date.ToTimestamp();

            // Assert
            Assert.AreEqual(3666815825U, value);
        }

        [TestMethod]
        public void ULong_to_DateTime()
        {
            // Arrange
            var epoch = DateTimeExtensions.UnixEpoch;
            var value = 987654321UL;

            // Act
            var date = value.ToDateTime();

            // Assert
            Assert.AreEqual(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), epoch);
            Assert.AreEqual(new DateTime(2001, 4, 19, 4, 25, 21, DateTimeKind.Utc), date);
        }

        [TestMethod]
        public void UInt_to_DateTime()
        {
            // Arrange
            var value = 987654321U;

            // Act
            var date = value.ToDateTime();

            // Assert
            Assert.AreEqual(new DateTime(2001, 4, 19, 4, 25, 21, DateTimeKind.Utc), date);
        }
    }
}