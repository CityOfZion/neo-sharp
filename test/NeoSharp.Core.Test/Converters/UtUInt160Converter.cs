using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Converters
{
    [TestClass]
    public class UtUInt160Converter
    {
        // Arrange
        private readonly UInt160 _value = UInt160.Parse("0x056291f871da523bff0e317e69ff7a42083cb39d");
        private TypeConverter _converter = TypeDescriptor.GetConverter(typeof(UInt160));

        [TestMethod]
        public void Can_Convert_To()
        {
            // Assert
            Assert.IsTrue(_converter.CanConvertTo(typeof(UInt160)));
            Assert.IsTrue(_converter.CanConvertTo(typeof(byte[])));
            Assert.IsTrue(_converter.CanConvertTo(typeof(string)));
            Assert.IsFalse(_converter.CanConvertTo(typeof(int)));
        }

        [TestMethod]
        public void Can_Convert_From()
        {
            // Assert
            Assert.IsTrue(_converter.CanConvertFrom(typeof(UInt160)));
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
            var result_ui160 = _converter.ConvertTo(_value, typeof(UInt160));
            var result_null = _converter.ConvertTo(null, typeof(UInt160));

            // Assert
            Assert.AreEqual(typeof(string), result_string.GetType());
            Assert.AreEqual(typeof(byte[]), result_bytearray.GetType());
            Assert.AreEqual(typeof(UInt160), result_ui160.GetType());
            Assert.AreEqual(_value, _converter.ConvertFrom(result_string));
            Assert.AreEqual(_value, _converter.ConvertFrom(result_bytearray));
            Assert.AreEqual(_value, result_ui160);
            Assert.IsNull(result_null);
        }


        [TestMethod]
        public void Serialize_Deserialize()
        {
            // Arrange
            BinarySerializer.RegisterTypes(typeof(UInt160));
            var _serializer = new BinarySerializer();
            var result = new byte[] {
                157, 179, 60, 8, 66, 122, 255, 105, 126, 49,
                14, 255, 59, 82, 218, 113, 248, 145, 98, 5};

            // Act
            var ret = _serializer.Serialize(_value);
            var clone = _serializer.Deserialize<UInt160>(ret);

            // Assert
            CollectionAssert.AreEqual(result, ret);
            Assert.AreEqual(_value, clone);
        }
    }
}
