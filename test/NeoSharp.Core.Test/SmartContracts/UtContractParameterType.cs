using System.Linq;
using System.Numerics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.SmartContract.ContractParameters;
using NeoSharp.Cryptography;
using NeoSharp.TestHelpers;
using NeoSharp.Types;
using NeoSharp.VM;

namespace NeoSharp.Core.Test.SmartContracts
{
    [TestClass]
    public class UtContractParameterTypeTests : TestBase
    {
        #region TestBooleanContractParameter

        [TestMethod]
        public void TestBooleanContractParameterSuccess()
        {
            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var boolContractParameter = new BooleanContractParameter(false);
            boolContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(false));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestBooleanContractParameterUnsuccess()
        {
            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var boolContractParameter = new BooleanContractParameter(false);
            boolContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(true));
        }

        #endregion

        #region TestByteArrayContractParameter

        [TestMethod]
        public void TestByteArrayContractParameterSuccess()
        {
            var expectedArray = Crypto.Default.GenerateRandomBytes(32);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var byteContractParameter = new ByteArrayContractParameter(expectedArray);
            byteContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(It.Is<byte[]>(b => b.SequenceEqual(expectedArray))));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestByteArrayContractParameterUnsuccess()
        {
            var expectedArray = Crypto.Default.GenerateRandomBytes(32);
            var notExpectedArray = Crypto.Default.GenerateRandomBytes(32);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var byteContractParameter = new ByteArrayContractParameter(expectedArray);
            byteContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(It.Is<byte[]>(b => b.SequenceEqual(notExpectedArray))));
        }

        #endregion

        #region TestHash160ContractParameter

        [TestMethod]
        public void TestHash160ContractParameterSuccess()
        {
            var uint160 = new UInt160();

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var uint160ContractParameter = new Hash160ContractParameter(uint160);
            uint160ContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(It.Is<byte[]>(b => b.SequenceEqual(uint160.ToArray()))));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestHash160ContractParameterUnsuccess()
        {
            var uint160 = new UInt160();
            var notExpectedValue = new UInt160(Crypto.Default.GenerateRandomBytes(20));

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var uint160ContractParameter = new Hash160ContractParameter(uint160);
            uint160ContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(It.Is<byte[]>(b => b.SequenceEqual(notExpectedValue.ToArray()))));
        }

        #endregion

        #region TestHash256ContractParameter

        [TestMethod]
        public void TestHash256ContractParameterSuccess()
        {
            var uint256 = new UInt256();

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var uint256ContractParameter = new Hash256ContractParameter(uint256);
            uint256ContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(It.Is<byte[]>(b => b.SequenceEqual(uint256.ToArray()))));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestHash256ContractParameterUnsuccess()
        {
            var uint256 = new UInt256();
            var notExpectedValue = new UInt256(Crypto.Default.GenerateRandomBytes(32));

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var uint256ContractParameter = new Hash256ContractParameter(uint256);
            uint256ContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(It.Is<byte[]>(b => b.SequenceEqual(notExpectedValue.ToArray()))));
        }

        #endregion

        #region TestIntegerContractParameter

        [TestMethod]
        public void TestIntegerContractParameterSuccess()
        {
            int integer = int.MaxValue;
            BigInteger expectedBigValue;
            BigInteger.TryParse(int.MaxValue.ToString(), out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var integerContractParameter = new IntegerContractParameter(integer);
            integerContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestIntegerContractParameterUnsuccess()
        {
            int integer = int.MaxValue;
            BigInteger expectedBigValue;
            BigInteger.TryParse("2", out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var integerContractParameter = new IntegerContractParameter(integer);
            integerContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        #endregion

        #region TestUIntContractParameter

        [TestMethod]
        public void TestUIntContractParameterSuccess()
        {
            uint uIntVariable = uint.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse(uint.MaxValue.ToString(), out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var uintegerContractParameter = new IntegerContractParameter(uIntVariable);
            uintegerContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestUIntContractParameterUnsuccess()
        {
            uint uIntVariable = uint.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse("2", out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var uintegerContractParameter = new IntegerContractParameter(uIntVariable);
            uintegerContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        #endregion

        #region TestShortContractParameter

        [TestMethod]
        public void TestShortContractParameterSuccess()
        {
            short shortVariable = short.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse(short.MaxValue.ToString(), out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var shortContractParameter = new IntegerContractParameter(shortVariable);
            shortContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestShortContractParameterUnsuccess()
        {
            short shortVariable = short.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse("2", out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var shortContractParameter = new IntegerContractParameter(shortVariable);
            shortContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        #endregion

        #region TestUShortContractParameter

        [TestMethod]
        public void TestUShortContractParameterSuccess()
        {
            ushort uShortVariable = ushort.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse(ushort.MaxValue.ToString(), out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var ushortContractParameter = new IntegerContractParameter(uShortVariable);
            ushortContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestUShortContractParameterUnsuccess()
        {
            ushort uShortVariable = ushort.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse("2", out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var ushortContractParameter = new IntegerContractParameter(uShortVariable);
            ushortContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        #endregion

        #region TestLongContractParameter

        [TestMethod]
        public void TestLongContractParameterSuccess()
        {
            long longVariable = long.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse(long.MaxValue.ToString(), out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var longContractParameter = new IntegerContractParameter(longVariable);
            longContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestLongContractParameterUnsuccess()
        {
            long longVariable = long.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse("2", out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var longContractParameter = new IntegerContractParameter(longVariable);
            longContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        #endregion

        #region TestULongContractParameter

        [TestMethod]
        public void TestULongContractParameterSuccess()
        {
            ulong uLongVariable = ulong.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse(ulong.MaxValue.ToString(), out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var ulongContractParameter = new IntegerContractParameter(uLongVariable);
            ulongContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestULongContractParameterUnsuccess()
        {
            ulong uLongVariable = ulong.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse("2", out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var ulongContractParameter = new IntegerContractParameter(uLongVariable);
            ulongContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        #endregion

        #region TestByteContractParameter

        [TestMethod]
        public void TestByteContractParameterSuccess()
        {
            byte byteVariable = byte.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse(byte.MaxValue.ToString(), out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var byteContractParameter = new IntegerContractParameter(byteVariable);
            byteContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestByteContractParameterUnsuccess()
        {
            byte byteVariable = byte.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse("2", out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var byteContractParameter = new IntegerContractParameter(byteVariable);
            byteContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        #endregion

        #region TestSByteContractParameter

        [TestMethod]
        public void TestSByteContractParameterSuccess()
        {
            sbyte signedByteVariable = sbyte.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse(sbyte.MaxValue.ToString(), out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var sbyteContractParameter = new IntegerContractParameter(signedByteVariable);
            sbyteContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestSByteContractParameterUnsuccess()
        {
            sbyte signedByteVariable = sbyte.MaxValue;

            BigInteger expectedBigValue;
            BigInteger.TryParse("2", out expectedBigValue);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var sbyteContractParameter = new IntegerContractParameter(signedByteVariable);
            sbyteContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        #endregion

        #region TestBigIntegerContractParameter

        [TestMethod]
        public void TestBigIntegerContractParameterSuccess()
        {
            BigInteger bigIntegerVariable = BigInteger.One;

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var bigintegerContractParameter = new IntegerContractParameter(bigIntegerVariable);
            bigintegerContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(bigIntegerVariable));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestBigIntegerContractParameterUnsuccess()
        {
            BigInteger bigIntegerVariable = BigInteger.One;
            BigInteger expectedBigValue = BigInteger.Zero;

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var bigintegerContractParameter = new IntegerContractParameter(bigIntegerVariable);
            bigintegerContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedBigValue));
        }

        #endregion

        #region TestStringContractParameter

        [TestMethod]
        public void TestStringContractParameterSuccess()
        {
            var randomString = RandomString(50);
            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var bigintegerContractParameter = new StringContractParameter(randomString);
            bigintegerContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(randomString));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestStringContractParameterUnsuccess()
        {
            var randomString = RandomString(50);
            var notexpected = RandomString(50);
            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var bigintegerContractParameter = new StringContractParameter(randomString);
            bigintegerContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(notexpected));
        }

        #endregion

        #region TestSignatureContractParameter

        [TestMethod]
        public void TestSignatureContractParameterSuccess()
        {
            var expectedArray = Crypto.Default.GenerateRandomBytes(32);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var signatureContractParameter = new SignatureContractParameter(expectedArray);
            signatureContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(It.Is<byte[]>(b => b.SequenceEqual(expectedArray))));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestSignatureContractParameterUnsuccess()
        {
            var expectedArray = Crypto.Default.GenerateRandomBytes(32);
            var notExpectedArray = Crypto.Default.GenerateRandomBytes(32);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var signatureContractParameter = new SignatureContractParameter(expectedArray);
            signatureContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(It.Is<byte[]>(b => b.SequenceEqual(notExpectedArray))));
        }

        #endregion

        #region TestPublicKeyContractParameter

        [TestMethod]
        public void TestPublicKeyContractParameterSuccess()
        {
            var privateKey = Crypto.Default.GenerateRandomBytes(32);
            var publicKey = Crypto.Default.ComputePublicKey(privateKey, true);
            var expectedPk = new ECPoint(publicKey);

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var publicKeyContractParameter = new PublicKeyContractParameter(expectedPk);
            publicKeyContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(expectedPk.EncodedData));
        }

        [TestMethod]
        [ExpectedException(typeof(MockException))]
        public void TestPublicKeyContractParameterUnsuccess()
        {
            var privateKey = Crypto.Default.GenerateRandomBytes(32);
            var publicKey = Crypto.Default.ComputePublicKey(privateKey, true);
            var expectedPk = new ECPoint(publicKey);

            var notExpectedPk = ECPoint.Infinity;

            var mockBuilder = new Mock<ScriptBuilder>();
            var item = mockBuilder.Object;

            var publicKeyContractParameter = new PublicKeyContractParameter(expectedPk);
            publicKeyContractParameter.PushIntoScriptBuilder(item);

            mockBuilder.Verify(m => m.EmitPush(notExpectedPk.EncodedData));
        }

        #endregion
    }
}
