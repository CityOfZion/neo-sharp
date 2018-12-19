using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.SmartContract;
using NeoSharp.Cryptography;
using NeoSharp.TestHelpers;
using NeoSharp.Types;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Test.SmartContracts
{
    [TestClass]
    public class UtContractFactory : TestBase
    {
        [TestMethod]
        public void TestCreateSinglePublicKeyRedeemContract()
        {
            var privateKey = new byte[]
            {
                103, 53, 97, 56, 98, 18, 129, 236, 104, 172, 110, 254, 31, 5, 142, 188, 116, 114, 216, 54, 247, 77, 151,
                243, 131, 155, 198, 39, 22, 111, 56, 126
            };

            var publicKey = Crypto.Default.ComputePublicKey(privateKey, true);
            var publicKeyInEcPoint = new ECPoint(publicKey);
            var result = ContractFactory.CreateSinglePublicKeyRedeemContract(publicKeyInEcPoint);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Parameters.Length);
            Assert.AreEqual(ContractParameterType.Signature, result.Parameters[0]);
            Assert.AreEqual(ContractParameterType.Void, result.ReturnType);
            Assert.AreEqual(UInt160.Parse("0x0d7c041c584acea51d66f1e9bf144477ad6dca25"), result.ScriptHash);
        }

        [TestMethod]
        public void TestCreateMultiplePublicKeyRedeemContract()
        {
            var pubKeys = new string[]
            {
                  "03b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c",
                  "02df48f60e8f3e01c48ff40b9b7f1310d7a8b2a193188befe1c2e3df740e895093",
                  "03b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04a",
                  "02ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba554",
                  "024c7b7fb6c310fccf1ba33b082519d82964ea93868d676662d4a59ad548df0e7d",
                  "02aaec38470f6aad0042c6e877cfd8087d2676b0f516fddd362801b9bd3936399e",
                  "02486fd15702c4490a26703112a5cc1d0923fd697a33406bd5a1c00e0013b09a70"
            };

            var publicKeyInEcPoints = pubKeys
                .Select(pk => new ECPoint(Crypto.Default.DecodePublicKey(pk.HexToBytes(), true, out var x, out var y)))
                .ToArray();

            Assert.ThrowsException<ArgumentException>(() => (ContractFactory.CreateMultiplePublicKeyRedeemContract(0, publicKeyInEcPoints)));
            Assert.ThrowsException<ArgumentException>(() => (ContractFactory.CreateMultiplePublicKeyRedeemContract(9, publicKeyInEcPoints)));

            var result = ContractFactory.CreateMultiplePublicKeyRedeemContract(3, publicKeyInEcPoints);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Parameters.Length);
            Assert.AreEqual(ContractParameterType.Signature, result.Parameters[0]);
            Assert.AreEqual(ContractParameterType.Void, result.ReturnType);
            Assert.AreEqual(UInt160.Parse("0x42096369ecec010e0932cb201e8b676769bb751d"), result.ScriptHash);
        }
    }
}