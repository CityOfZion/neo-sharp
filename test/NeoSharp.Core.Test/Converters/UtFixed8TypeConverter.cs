using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Converters
{
    [TestClass]
    public class UtFixed8TypeConverter
    {
        // Arrange
        private readonly Fixed8 _value = Fixed8.Parse("6527654786");
        TypeConverter _converter = TypeDescriptor.GetConverter(typeof(Fixed8));

        [TestMethod]
        public void Can_Convert_To()
        {
            // Assert
            Assert.IsTrue(_converter.CanConvertTo(typeof(Fixed8)));
            Assert.IsTrue(_converter.CanConvertTo(typeof(byte[])));
            Assert.IsTrue(_converter.CanConvertTo(typeof(string)));
            Assert.IsFalse(_converter.CanConvertTo(typeof(int)));
        }

        [TestMethod]
        public void Can_Convert_From()
        {
            // Assert
            Assert.IsTrue(_converter.CanConvertFrom(typeof(Fixed8)));
            Assert.IsTrue(_converter.CanConvertFrom(typeof(byte[])));
            Assert.IsTrue(_converter.CanConvertFrom(typeof(string)));
            Assert.IsFalse(_converter.CanConvertFrom(typeof(int)));
        }

        [TestMethod]
        public void Conversions()
        {
            // Act
            var result_string = _converter.ConvertTo(_value, typeof(string));
            var result_bytearray = _converter.ConvertTo(_value, typeof(byte[]));
            var result_f8 = _converter.ConvertTo(_value, typeof(Fixed8));
            var result_null = _converter.ConvertTo(null, typeof(Fixed8));

            // Assert
            Assert.AreEqual(typeof(string), result_string.GetType());
            Assert.AreEqual(typeof(byte[]), result_bytearray.GetType());
            Assert.AreEqual(typeof(Fixed8), result_f8.GetType());
            Assert.AreEqual(_value, _converter.ConvertFrom(result_string));
            Assert.AreEqual(_value, _converter.ConvertFrom(result_bytearray));
            Assert.AreEqual(_value, result_f8);
            Assert.IsNull(result_null);
        }

        [TestMethod]
        public void Serialize_Deserialize()
        {
            // Arrange
            BinarySerializer.RegisterTypes(typeof(Fixed8));
            var _serializer = new BinarySerializer();
            var result = new byte[] { 0, 66, 151, 137, 190, 22, 15, 9};

            // Act
            var ret = _serializer.Serialize(_value);
            var clone = _serializer.Deserialize<Fixed8>(ret);

            // Assert
            CollectionAssert.AreEqual(result, ret);
            Assert.AreEqual(_value, clone);
        }
    }
}
