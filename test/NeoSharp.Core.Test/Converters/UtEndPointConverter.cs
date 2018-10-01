using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Test.Converters
{
    [TestClass]
    public class UtEndPointConverter
    {
        // Arrange
        EndPoint _value = new EndPoint();
        TypeConverter _converter = TypeDescriptor.GetConverter(typeof(EndPoint));

        [TestInitialize]
        public void Init()
        {
            _value.Protocol = Protocol.Tcp;
            _value.Host = "11.22.33.44";
            _value.Port = 10332;
        }

        [TestMethod]
        public void Can_Convert_To()
        {
            // Assert
            Assert.IsTrue(_converter.CanConvertTo(typeof(EndPoint)));
            Assert.IsTrue(_converter.CanConvertTo(typeof(byte[])));
            Assert.IsTrue(_converter.CanConvertTo(typeof(string)));
            Assert.IsFalse(_converter.CanConvertTo(typeof(int)));
        }

        [TestMethod]
        public void Can_Convert_From()
        {
            // Assert
            Assert.IsTrue(_converter.CanConvertFrom(typeof(EndPoint)));
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
            var result_endpoint = _converter.ConvertTo(_value, typeof(EndPoint));
            var result_null = _converter.ConvertTo(null, typeof(EndPoint));

            // Assert
            Assert.AreEqual(typeof(string), result_string.GetType());
            Assert.AreEqual(typeof(byte[]), result_bytearray.GetType());
            Assert.AreEqual(typeof(EndPoint), result_endpoint.GetType());
            Assert.AreEqual(_value, _converter.ConvertFrom(result_string));
            Assert.AreEqual(_value, _converter.ConvertFrom(result_bytearray));
            Assert.AreEqual(_value, result_endpoint);
            Assert.IsNull(result_null);
        }

        [TestMethod]
        public void Serialize_Deserialize()
        {
            // Arrange
            BinarySerializer.RegisterTypes(typeof(EndPoint));
            var _serializer = new BinarySerializer();
            var result = new byte[] {
                23, 116, 99, 112, 58, 47, 47, 49, 49, 46, 50, 50,
                46, 51, 51, 46, 52, 52, 58, 49, 48, 51, 51, 50};

            // Act
            var ret = _serializer.Serialize(_value);
            var clone = _serializer.Deserialize<EndPoint>(ret);

            // Assert
            CollectionAssert.AreEqual(result, ret);
            Assert.AreEqual(_value, clone);
        }
    }
}
