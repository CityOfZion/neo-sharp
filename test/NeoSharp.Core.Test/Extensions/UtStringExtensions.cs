using System;
using System.Linq;
using System.Security;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Test.Extensions
{
    [TestClass]
    public class UtStringExtensions
    {
        [TestMethod]
        public void CommandToToken()
        {
            // unquoted

            var command = "sum";
            var cmdArgs = command.SplitCommandLine().ToArray();

            CollectionAssert.AreEqual(new CommandToken[] { new CommandToken("sum", 0, 3) }, cmdArgs);

            // quoted

            command = "\"sum numbers\"  1 2";
            cmdArgs = command.SplitCommandLine().ToArray();

            CollectionAssert.AreEqual(new CommandToken[] { new CommandToken("\"sum numbers\"", 0, 13), new CommandToken("1", 15, 1), new CommandToken("2", 17, 1) }, cmdArgs);

            // quoted & escaping

            command = "\"sum \\\"numbers\\\"\" 1 2";
            cmdArgs = command.SplitCommandLine().ToArray();

            CollectionAssert.AreEqual(new CommandToken[] { new CommandToken("sum \"numbers\"", true, 0, 17), new CommandToken("1", 18, 1), new CommandToken("2", 20, 1) }, cmdArgs);
        }

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

            value.Should().BeEquivalentTo(new byte[] { 226, 130, 172 });

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

        [TestMethod]
        public void CheckHexStringUnsuccess()
        {
            var address = "ALhYzeoL7r7CLggtnyFpgF9kSxKuekWMGg";
            Assert.IsFalse(address.IsHexString());
        }

        [TestMethod]
        public void CheckHexStringSuccess()
        {
            var address = "736f6d6574657374";
            Assert.IsTrue(address.IsHexString());
        }

        [TestMethod]
        public void CheckHexStringSuccessWithZeroX()
        {
            var address = "0x736f6d6574657374";
            Assert.IsTrue(address.IsHexString());
        }
    }
}