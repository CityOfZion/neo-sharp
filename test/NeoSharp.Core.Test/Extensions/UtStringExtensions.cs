using System;
using System.Security;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtStringExtensions
    {
        [TestMethod]
        public void Can_convert_null_hex_string_to_bytes()
        {
            var value = default(string).HexToBytes();

            value.Should().BeEquivalentTo(new byte[0]);
        }

        [TestMethod]
        public void Can_convert_empty_hex_string_to_bytes()
        {
            var value = string.Empty.HexToBytes();

            value.Should().BeEquivalentTo(new byte[0]);
        }

        [TestMethod]
        public void Can_convert_hex_string_starting_with_0x_to_bytes()
        {
            var value = "0x".HexToBytes();

            value.Should().BeEquivalentTo(new byte[0]);
        }

        [TestMethod]
        public void Throw_if_hex_string_is_not_even()
        {
            Action a = () => "1".HexToBytes();

            a.Should().Throw<FormatException>();
        }

        [TestMethod]
        public void Throw_if_hex_string_exceeds_expected_limit()
        {
            Action a = () => "1".HexToBytes(2);

            a.Should().Throw<FormatException>();
        }

        [TestMethod]
        public void Can_convert_lowercase_hex_string_to_bytes()
        {
            var value = "ff".HexToBytes();

            value.Should().BeEquivalentTo(new byte[] { 255 });
        }

        [TestMethod]
        public void Can_convert_uppercase_hex_string_to_bytes()
        {
            var value = "FF".HexToBytes();

            value.Should().BeEquivalentTo(new byte[] { 255 });
        }

        [TestMethod]
        public void SecureStringToByteArray()
        {
            var secureS = new SecureString();
            secureS.AppendChar('x');
            var value = secureS.ToByteArray();

            value.Should().BeEquivalentTo(new byte[] { 120 });
        }

        [TestMethod]
        public void SecureStringToByteArrayWithEuroSymbol()
        {
            var secureS = new SecureString();
            secureS.AppendChar('€');
            var value = secureS.ToByteArray();

            value.Should().BeEquivalentTo(new byte[] { 226,  130, 172 });

            var str = Encoding.UTF8.GetString(value);
            str.Should().Equals("€");
        }

        [TestMethod]
        public void SecureStringToByteArrayWithChineseChar()
        {
            var secureS = new SecureString();
            secureS.AppendChar('小');
            var value = secureS.ToByteArray();

            value.Should().BeEquivalentTo(new byte[] { 229, 176, 143 });

            var str = Encoding.UTF8.GetString(value);
            Assert.IsTrue(str.Equals("小"));
        }
    }
}