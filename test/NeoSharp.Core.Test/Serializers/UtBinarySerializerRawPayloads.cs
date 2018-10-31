using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Types;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Test.Serializers
{
    [TestClass]
    public class UtBinarySerializerRawPayloads
    {
        private IBinarySerializer _serializer;
        private ISigner<Witness> _signer;

        [TestInitialize]
        public void WarmUpSerializer()
        {
            _serializer = new BinarySerializer(typeof(BlockHeader).Assembly, typeof(UtBinarySerializer).Assembly, typeof(Fixed8).Assembly);
            _signer = new WitnessOperationsManager(NeoSharp.Cryptography.Crypto.Default);
        }

        [TestMethod]
        public void SerializeDeserialize_ConsensusPayload()
        {
            // Real message provided by neo-cli

            var data = "0000000033366e89d9647dc4e6e8389b4f057c4b6f24936fa67c1e32f12ef100b1532b367f702c0004002a93d95b422100331753c0407b455c2a8bc1f3922b229f29580f37c6bc08226b54fd897df30c5cf482ee3796e329ce9aee41c8eadec550ad7f707cff3d404307428e8d31b090a5014140ea19f32db0d9a4983508e7b419ecc64771bdb8417947ccac862053015bdb53adc1790345b9e65feb89b96cffcb775462138d34408c2132416c43e3badd08ebcd232103b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04aac".HexToBytes();
            var payload = (ConsensusPayload)_serializer.Deserialize<ConsensusPayload>(data);

            Assert.AreEqual(payload.Unsigned.Version, 0U);
            Assert.AreEqual(payload.Unsigned.PrevHash.ToString(true), "0x362b53b100f12ef1321e7ca66f93246f4b7c054f9b38e8e6c47d64d9896e3633");
            Assert.AreEqual(payload.Unsigned.BlockIndex, 2912383U);
            Assert.AreEqual(payload.Unsigned.ValidatorIndex, (ushort)4);
            Assert.AreEqual(payload.Unsigned.Timestamp, 1540985642U);
            Assert.AreEqual(payload.Unsigned.Data.ToHexString(true), "0x2100331753c0407b455c2a8bc1f3922b229f29580f37c6bc08226b54fd897df30c5cf482ee3796e329ce9aee41c8eadec550ad7f707cff3d404307428e8d31b090a5");

            _signer.Sign(payload.Script);

            Assert.AreEqual(payload.Script.Hash.ToString(true), "0xcd77750080d4a7e5982fdc1a3d71444e520e28b1");
            Assert.AreEqual(payload.Script.InvocationScript.ToHexString(true), "0x40ea19f32db0d9a4983508e7b419ecc64771bdb8417947ccac862053015bdb53adc1790345b9e65feb89b96cffcb775462138d34408c2132416c43e3badd08ebcd");
            Assert.AreEqual(payload.Script.VerificationScript.ToHexString(true), "0x2103b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04aac");
        }
    }
}