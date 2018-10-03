using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Converters
{
    [TestClass]
    public class UtUInt256Converter
    {
        // Arrange
        private readonly UInt256 _value = UInt256.Parse("0x4520462a8c80056291f871da523bff0eb17e29d44ab4317e69ff7a42083cb39d");
        private readonly TypeConverter _converter = TypeDescriptor.GetConverter(typeof(UInt256));


        [TestMethod]
        public void Can_Convert_To()
        {
            // Assert
            Assert.IsTrue(_converter.CanConvertTo(typeof(UInt256)));
            Assert.IsTrue(_converter.CanConvertTo(typeof(byte[])));
            Assert.IsTrue(_converter.CanConvertTo(typeof(string)));
            Assert.IsFalse(_converter.CanConvertTo(typeof(int)));
        }

        [TestMethod]
        public void Can_Convert_From()
        {
            // Assert
            Assert.IsTrue(_converter.CanConvertFrom(typeof(UInt256)));
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
            var result_ui256 = _converter.ConvertTo(_value, typeof(UInt256));
            var result_null = _converter.ConvertTo(null, typeof(UInt256));

            // Assert
            Assert.AreEqual(typeof(string), result_string.GetType());
            Assert.AreEqual(typeof(byte[]), result_bytearray.GetType());
            Assert.AreEqual(typeof(UInt256), result_ui256.GetType());
            Assert.AreEqual(_value, _converter.ConvertFrom(result_string));
            Assert.AreEqual(_value, _converter.ConvertFrom(result_bytearray));
            Assert.AreEqual(_value, result_ui256);
            Assert.IsNull(result_null);
        }


        [TestMethod]
        public void Serialize_Deserialize()
        {
            // Arrange
            BinarySerializer.RegisterTypes(typeof(UInt256));
            var _serializer = new BinarySerializer();
            var result = new byte[] {
                157, 179, 60, 8, 66, 122, 255, 105, 126, 49, 180, 74, 212, 41, 126, 177,
                14, 255, 59, 82, 218, 113, 248, 145, 98, 5, 128, 140, 42, 70, 32, 69};

            // Act
            var ret = _serializer.Serialize(_value);
            var clone = _serializer.Deserialize<UInt256>(ret);

            // Assert
            CollectionAssert.AreEqual(result, ret);
            Assert.AreEqual(_value, clone);
        }
    }
}