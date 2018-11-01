using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.SmartContract;
using NeoSharp.Cryptography;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

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
    }
}
