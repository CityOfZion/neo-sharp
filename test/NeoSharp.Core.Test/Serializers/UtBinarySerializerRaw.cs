using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Test.Serializers
{
    [TestClass]
    public class UtBinarySerializerRaw
    {
        private ICrypto _crypto;
        private IBinarySerializer _serializer;
        private IBinaryDeserializer _deserializer;

        [TestInitialize]
        public void WarmUpSerializer()
        {
            _crypto = new BouncyCastleCrypto();
            _serializer = new BinarySerializer(typeof(BlockHeader).Assembly, typeof(UtBinarySerializer).Assembly);
            _deserializer = new BinaryDeserializer(typeof(BlockHeader).Assembly, typeof(UtBinarySerializer).Assembly);
        }

        [TestMethod]
        public void Raw_Block()
        {
            // MainNet - Block 1

            var data = "00000000bf4421c88776c53b43ce1dc45463bfd2028e322fdfb60064be150ed3e36125d418f98ec3ed2c2d1c9427385e7b85d0d1a366e29c4e399693a59718380f8bbad6d6d90358010000004490d0bb7170726c59e75d652b5d3827bf04c165bbe9ef95cca4bf5501fd4501404edf5005771de04619235d5a4c7a9a11bb78e008541f1da7725f654c33380a3c87e2959a025da706d7255cb3a3fa07ebe9c6559d0d9e6213c68049168eb1056f4038a338f879930c8adc168983f60aae6f8542365d844f004976346b70fb0dd31aa1dbd4abd81e4a4aeef9941ecd4e2dd2c1a5b05e1cc74454d0403edaee6d7a4d4099d33c0b889bf6f3e6d87ab1b11140282e9a3265b0b9b918d6020b2c62d5a040c7e0c2c7c1dae3af9b19b178c71552ebd0b596e401c175067c70ea75717c8c00404e0ebd369e81093866fe29406dbf6b402c003774541799d08bf9bb0fc6070ec0f6bad908ab95f05fa64e682b485800b3c12102a8596e6c715ec76f4564d5eff34070e0521979fcd2cbbfa1456d97cc18d9b4a6ad87a97a2a0bcdedbf71b6c9676c645886056821b6f3fec8694894c66f41b762bc4e29e46ad15aee47f05d27d822f1552102486fd15702c4490a26703112a5cc1d0923fd697a33406bd5a1c00e0013b09a7021024c7b7fb6c310fccf1ba33b082519d82964ea93868d676662d4a59ad548df0e7d2102aaec38470f6aad0042c6e877cfd8087d2676b0f516fddd362801b9bd3936399e2103b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c2103b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04a2102ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba5542102df48f60e8f3e01c48ff40b9b7f1310d7a8b2a193188befe1c2e3df740e89509357ae0100004490d0bb00000000".HexToBytes();
            var block = _deserializer.Deserialize<Block>(data);

            Assert.AreEqual(block.Version, 0U);
            Assert.AreEqual(block.PreviousBlockHash.ToString(true), "0xd42561e3d30e15be6400b6df2f328e02d2bf6354c41dce433bc57687c82144bf");
            Assert.AreEqual(block.MerkleRoot.ToString(true), "0xd6ba8b0f381897a59396394e9ce266a3d1d0857b5e3827941c2d2cedc38ef918");
            Assert.AreEqual(block.Timestamp, 1476647382U);
            Assert.AreEqual(block.Index, 1U);
            Assert.AreEqual(block.ConsensusData, 7814431937225855044UL);
            Assert.AreEqual(block.NextConsensus.ToString(true), "0x55bfa4cc95efe9bb65c104bf27385d2b655de759");

            block.UpdateHash(_serializer, _crypto);

            Assert.AreEqual(block.Script.InvocationScript.ToHexString(true), "0x404edf5005771de04619235d5a4c7a9a11bb78e008541f1da7725f654c33380a3c87e2959a025da706d7255cb3a3fa07ebe9c6559d0d9e6213c68049168eb1056f4038a338f879930c8adc168983f60aae6f8542365d844f004976346b70fb0dd31aa1dbd4abd81e4a4aeef9941ecd4e2dd2c1a5b05e1cc74454d0403edaee6d7a4d4099d33c0b889bf6f3e6d87ab1b11140282e9a3265b0b9b918d6020b2c62d5a040c7e0c2c7c1dae3af9b19b178c71552ebd0b596e401c175067c70ea75717c8c00404e0ebd369e81093866fe29406dbf6b402c003774541799d08bf9bb0fc6070ec0f6bad908ab95f05fa64e682b485800b3c12102a8596e6c715ec76f4564d5eff34070e0521979fcd2cbbfa1456d97cc18d9b4a6ad87a97a2a0bcdedbf71b6c9676c645886056821b6f3fec8694894c66f41b762bc4e29e46ad15aee47f05d27d822");
            Assert.AreEqual(block.Script.VerificationScript.ToHexString(true), "0x552102486fd15702c4490a26703112a5cc1d0923fd697a33406bd5a1c00e0013b09a7021024c7b7fb6c310fccf1ba33b082519d82964ea93868d676662d4a59ad548df0e7d2102aaec38470f6aad0042c6e877cfd8087d2676b0f516fddd362801b9bd3936399e2103b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c2103b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04a2102ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba5542102df48f60e8f3e01c48ff40b9b7f1310d7a8b2a193188befe1c2e3df740e89509357ae");
            Assert.AreEqual(block.Script.Hash.ToString(true), "0x55bfa4cc95efe9bb65c104bf27385d2b655de759");

            Assert.AreEqual(block.Hash.ToString(true), "0xd782db8a38b0eea0d7394e0f007c61c71798867578c77c387c08113903946cc9");

            Assert.AreEqual(block.Transactions.Length, 1);

            MinerTransaction tx = block.Transactions[0] as MinerTransaction;

            tx.UpdateHash(_serializer, _crypto);

            Assert.AreEqual(tx.Hash.ToString(true), "0xd6ba8b0f381897a59396394e9ce266a3d1d0857b5e3827941c2d2cedc38ef918");
            Assert.AreEqual(tx.Nonce, 3151007812);
            Assert.AreEqual(tx.Attributes.Length, 0);
            Assert.AreEqual(tx.Inputs.Length, 0);
            Assert.AreEqual(tx.Outputs.Length, 0);
        }

        [TestMethod]
        public void SerializeDeserialize_ClaimTransaction()
        {
            // Mainnet Block=4275 / Tx=1

            var data = "020001fda149910702cc19ed967c32f883a322f2e1713790c1398f538a42e489d485ee0000000001e72d286979ee6cb1b7e65dfddfb2e384100b8d148e7758de42e4168b71792c60c074110000000000f41cdd4b7ec41847443fa36bf8dde0009d7ecebc01414019fcb645e67b870a657fe028bcb057f866347d211dc26a25fe0570250f41d0c881113e1820ac55a029e6fc5acab80587f9bebf8b84dbd4503ba816c417b8bf522321039f07df7861c216de3b78c647b77f8b01404b400a437302b651cdf206ec1af626ac".HexToBytes();
            var tx = (ClaimTransaction)_deserializer.Deserialize<Transaction>(data);

            tx.UpdateHash(_serializer, _crypto);

            Assert.AreEqual(tx.Hash.ToString(true), "0x462c0e6fcd68853dd44f4055e2aa759548038d3b1362b6182398a6d44c0d1bf0");
            Assert.AreEqual(tx.Attributes.Length, 0);

            Assert.AreEqual(tx.Claims.Length, 1);
            Assert.AreEqual(tx.Claims[0].PrevHash.ToString(true), "0xee85d489e4428a538f39c1903771e1f222a383f8327c96ed19cc02079149a1fd");
            Assert.AreEqual(tx.Claims[0].PrevIndex, 0);

            Assert.AreEqual(tx.Scripts.Length, 1);
            Assert.AreEqual(tx.Scripts[0].Hash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Scripts[0].InvocationScript.ToHexString(true), "0x4019fcb645e67b870a657fe028bcb057f866347d211dc26a25fe0570250f41d0c881113e1820ac55a029e6fc5acab80587f9bebf8b84dbd4503ba816c417b8bf52");
            Assert.AreEqual(tx.Scripts[0].VerificationScript.ToHexString(true), "0x21039f07df7861c216de3b78c647b77f8b01404b400a437302b651cdf206ec1af626ac");

            Assert.AreEqual(tx.Inputs.Length, 0);

            Assert.AreEqual(tx.Outputs.Length, 1);
            Assert.AreEqual(tx.Outputs[0].AssetId.ToString(true), "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7");
            Assert.AreEqual(tx.Outputs[0].ScriptHash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Outputs[0].Value.ToString(), "0.01144");

            Assert.AreEqual(tx.Version, 0);
        }

        [TestMethod]
        public void SerializeDeserialize_ContractTransaction()
        {
            // Mainnet Block=4130 / Tx=1

            var data = "80000001da7c124f33b051fb04f95a02304837433499eb09087e3d035b6fca2460f631360000029b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc500e8764817000000f41cdd4b7ec41847443fa36bf8dde0009d7ecebc9b7cffdaa674beae0f930ebe6085af9093e5fe56b34a5c220ccdcf6efc336fc500184a27db8623009f2d1729a79436148dc442c25f41335ef9f78bbd01fd0401406a159b7552c7eaedc79abc86faeca7aa50af52aaa0f14aa9a4abaf498f270a140709992253df55de1b2fd93a6ea13b5344dacbd4e54e4f661fe073edeb72e2f740e28c0866c2ea963e40f8f6edbc1e40b76181fef43a4016d234602a52b31b83f02d745d57188cd72fcb1a8394a39d77270334374848266bb87a29fa4114d1b13240c1e7eae0e8e8d33b1a16c8ece8e96bc832d8f0a069499b8b9590609d8cd2a799a555f5433bdc153466bf6eefea0a568bd08b28afabfacb673785fe8d59ab82ea404874390b85c4d37d3645e03cae571000f3ca344452c2a4018aab57f73750dfb695c5488e3c9887699a2ff69e539b7e37278f470b03bc357ebaad25c397ef3104f1542102486fd15702c4490a26703112a5cc1d0923fd697a33406bd5a1c00e0013b09a7021024c7b7fb6c310fccf1ba33b082519d82964ea93868d676662d4a59ad548df0e7d2102aaec38470f6aad0042c6e877cfd8087d2676b0f516fddd362801b9bd3936399e2103b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c2103b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04a2102ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba5542102df48f60e8f3e01c48ff40b9b7f1310d7a8b2a193188befe1c2e3df740e89509357ae".HexToBytes();
            var tx = (ContractTransaction)_deserializer.Deserialize<Transaction>(data);

            tx.UpdateHash(_serializer, _crypto);

            Assert.AreEqual(tx.Hash.ToString(true), "0xee85d489e4428a538f39c1903771e1f222a383f8327c96ed19cc02079149a1fd");
            Assert.AreEqual(tx.Attributes.Length, 0);

            Assert.AreEqual(tx.Scripts.Length, 1);
            Assert.AreEqual(tx.Scripts[0].Hash.ToString(true), "0x1cfacc3e315977329c11ca50fe753730939da95f");
            Assert.AreEqual(tx.Scripts[0].InvocationScript.ToHexString(true), "0x406a159b7552c7eaedc79abc86faeca7aa50af52aaa0f14aa9a4abaf498f270a140709992253df55de1b2fd93a6ea13b5344dacbd4e54e4f661fe073edeb72e2f740e28c0866c2ea963e40f8f6edbc1e40b76181fef43a4016d234602a52b31b83f02d745d57188cd72fcb1a8394a39d77270334374848266bb87a29fa4114d1b13240c1e7eae0e8e8d33b1a16c8ece8e96bc832d8f0a069499b8b9590609d8cd2a799a555f5433bdc153466bf6eefea0a568bd08b28afabfacb673785fe8d59ab82ea404874390b85c4d37d3645e03cae571000f3ca344452c2a4018aab57f73750dfb695c5488e3c9887699a2ff69e539b7e37278f470b03bc357ebaad25c397ef3104");
            Assert.AreEqual(tx.Scripts[0].VerificationScript.ToHexString(true), "0x542102486fd15702c4490a26703112a5cc1d0923fd697a33406bd5a1c00e0013b09a7021024c7b7fb6c310fccf1ba33b082519d82964ea93868d676662d4a59ad548df0e7d2102aaec38470f6aad0042c6e877cfd8087d2676b0f516fddd362801b9bd3936399e2103b209fd4f53a7170ea4444e0cb0a6bb6a53c2bd016926989cf85f9b0fba17a70c2103b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04a2102ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba5542102df48f60e8f3e01c48ff40b9b7f1310d7a8b2a193188befe1c2e3df740e89509357ae");

            Assert.AreEqual(tx.Inputs.Length, 1);
            Assert.AreEqual(tx.Inputs[0].PrevHash.ToString(true), "0x3631f66024ca6f5b033d7e0809eb993443374830025af904fb51b0334f127cda");
            Assert.AreEqual(tx.Inputs[0].PrevIndex, 0);

            Assert.AreEqual(tx.Outputs.Length, 2);
            Assert.AreEqual(tx.Outputs[0].AssetId.ToString(true), "0xc56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b");
            Assert.AreEqual(tx.Outputs[0].ScriptHash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Outputs[0].Value.ToString(), "1000");

            Assert.AreEqual(tx.Outputs[1].AssetId.ToString(true), "0xc56f33fc6ecfcd0c225c4ab356fee59390af8560be0e930faebe74a6daff7c9b");
            Assert.AreEqual(tx.Outputs[1].ScriptHash.ToString(true), "0xbd8bf7f95e33415fc242c48d143694a729172d9f");
            Assert.AreEqual(tx.Outputs[1].Value.ToString(), "99999000");

            Assert.AreEqual(tx.Version, 0);
        }

        [TestMethod]
        public void SerializeDeserialize_IssueTransaction()
        {
            // Mainnet Block=12285 / Tx=3

            var data = "010000017ded1c83bd63e8871c8c2ad57607fe1423e8796606f2f5c2fe25be3f27f89a430000037ded1c83bd63e8871c8c2ad57607fe1423e8796606f2f5c2fe25be3f27f89a43001f8ed117000000f41cdd4b7ec41847443fa36bf8dde0009d7ecebc7ded1c83bd63e8871c8c2ad57607fe1423e8796606f2f5c2fe25be3f27f89a4300e1f5050000000055d6bc2c5a139c894df2344e03d1d2e1fbb7b609e72d286979ee6cb1b7e65dfddfb2e384100b8d148e7758de42e4168b71792c6040469af32a020000f41cdd4b7ec41847443fa36bf8dde0009d7ecebc014140420d9cdc020c525f95ae8464f7c51d0b84ee820e0073536a658f35428bd44e1941f4b1697a27cbdf3975da3366db6d3e6ec8e4aef3c50eff376a330bf728b5b42321039f07df7861c216de3b78c647b77f8b01404b400a437302b651cdf206ec1af626ac".HexToBytes();
            var tx = (IssueTransaction)_deserializer.Deserialize<Transaction>(data);

            tx.UpdateHash(_serializer, _crypto);

            Assert.AreEqual(tx.Hash.ToString(true), "0xf1ec2baf76c47bb3460369a0f962321d30423e1329d0c0734d9cd7fce8ed89c2");
            Assert.AreEqual(tx.Attributes.Length, 0);

            Assert.AreEqual(tx.Scripts.Length, 1);
            Assert.AreEqual(tx.Scripts[0].Hash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Scripts[0].InvocationScript.ToHexString(true), "0x40420d9cdc020c525f95ae8464f7c51d0b84ee820e0073536a658f35428bd44e1941f4b1697a27cbdf3975da3366db6d3e6ec8e4aef3c50eff376a330bf728b5b4");
            Assert.AreEqual(tx.Scripts[0].VerificationScript.ToHexString(true), "0x21039f07df7861c216de3b78c647b77f8b01404b400a437302b651cdf206ec1af626ac");

            Assert.AreEqual(tx.Inputs.Length, 1);
            Assert.AreEqual(tx.Inputs[0].PrevHash.ToString(true), "0x439af8273fbe25fec2f5f2066679e82314fe0776d52a8c1c87e863bd831ced7d");
            Assert.AreEqual(tx.Inputs[0].PrevIndex, 0);

            Assert.AreEqual(tx.Outputs.Length, 3);

            Assert.AreEqual(tx.Outputs[0].AssetId.ToString(true), "0x439af8273fbe25fec2f5f2066679e82314fe0776d52a8c1c87e863bd831ced7d");
            Assert.AreEqual(tx.Outputs[0].ScriptHash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Outputs[0].Value.ToString(), "1023");

            Assert.AreEqual(tx.Outputs[1].AssetId.ToString(true), "0x439af8273fbe25fec2f5f2066679e82314fe0776d52a8c1c87e863bd831ced7d");
            Assert.AreEqual(tx.Outputs[1].ScriptHash.ToString(true), "0x09b6b7fbe1d2d1034e34f24d899c135a2cbcd655");
            Assert.AreEqual(tx.Outputs[1].Value.ToString(), "1");

            Assert.AreEqual(tx.Outputs[2].AssetId.ToString(true), "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7");
            Assert.AreEqual(tx.Outputs[2].ScriptHash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Outputs[2].Value.ToString(), "23834.98856");

            Assert.AreEqual(tx.Version, 0);
        }

        [TestMethod]
        public void SerializeDeserialize_MinerTransaction()
        {
            // Mainnet Block=1 / tx=0

            var data = "00004490d0bb00000000".HexToBytes();
            var tx = (MinerTransaction)_deserializer.Deserialize<Transaction>(data);

            tx.UpdateHash(_serializer, _crypto);

            Assert.AreEqual(tx.Hash.ToString(true), "0xd6ba8b0f381897a59396394e9ce266a3d1d0857b5e3827941c2d2cedc38ef918");
            Assert.AreEqual(tx.Nonce, 3151007812);
            Assert.AreEqual(tx.Scripts.Length, 0);
            Assert.AreEqual(tx.Attributes.Length, 0);
            Assert.AreEqual(tx.Inputs.Length, 0);
            Assert.AreEqual(tx.Outputs.Length, 0);
            Assert.AreEqual(tx.Version, 0);
        }

        [TestMethod]
        public void SerializeDeserialize_InvocationTransaction()
        {
            // Mainnet Block= / Tx=

            var data = "".HexToBytes();
            var tx = (InvocationTransaction)_deserializer.Deserialize<Transaction>(data);

            tx.UpdateHash(_serializer, _crypto);

            Assert.AreEqual(tx.Hash.ToString(true), "0xf1ec2baf76c47bb3460369a0f962321d30423e1329d0c0734d9cd7fce8ed89c2");
            Assert.AreEqual(tx.Attributes.Length, 0);

            Assert.AreEqual(tx.Script.ToHexString(true), "");
            Assert.AreEqual(tx.Gas.ToString(), "");

            Assert.AreEqual(tx.Scripts.Length, 1);
            Assert.AreEqual(tx.Scripts[0].Hash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Scripts[0].InvocationScript.ToHexString(true), "0x40420d9cdc020c525f95ae8464f7c51d0b84ee820e0073536a658f35428bd44e1941f4b1697a27cbdf3975da3366db6d3e6ec8e4aef3c50eff376a330bf728b5b4");
            Assert.AreEqual(tx.Scripts[0].VerificationScript.ToHexString(true), "0x21039f07df7861c216de3b78c647b77f8b01404b400a437302b651cdf206ec1af626ac");

            Assert.AreEqual(tx.Inputs.Length, 1);
            Assert.AreEqual(tx.Inputs[0].PrevHash.ToString(true), "0x439af8273fbe25fec2f5f2066679e82314fe0776d52a8c1c87e863bd831ced7d");
            Assert.AreEqual(tx.Inputs[0].PrevIndex, 0);

            Assert.AreEqual(tx.Outputs.Length, 3);

            Assert.AreEqual(tx.Outputs[0].AssetId.ToString(true), "0x439af8273fbe25fec2f5f2066679e82314fe0776d52a8c1c87e863bd831ced7d");
            Assert.AreEqual(tx.Outputs[0].ScriptHash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Outputs[0].Value.ToString(), "1023");

            Assert.AreEqual(tx.Outputs[1].AssetId.ToString(true), "0x439af8273fbe25fec2f5f2066679e82314fe0776d52a8c1c87e863bd831ced7d");
            Assert.AreEqual(tx.Outputs[1].ScriptHash.ToString(true), "0x09b6b7fbe1d2d1034e34f24d899c135a2cbcd655");
            Assert.AreEqual(tx.Outputs[1].Value.ToString(), "1");

            Assert.AreEqual(tx.Outputs[2].AssetId.ToString(true), "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7");
            Assert.AreEqual(tx.Outputs[2].ScriptHash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Outputs[2].Value.ToString(), "23834.98856");

            Assert.AreEqual(tx.Version, 0);
        }

        [TestMethod]
        public void SerializeDeserialize_RegisterTransaction()
        {
            // Mainnet Block=4329 / Tx=1

            var data = "400060335b7b226c616e67223a227a682d434e222c226e616d65223a2248656c6c6f20416e74536861726573204d61696e6e6574227d5d000084d71700000008039f07df7861c216de3b78c647b77f8b01404b400a437302b651cdf206ec1af626f41cdd4b7ec41847443fa36bf8dde0009d7ecebc0001b3ba761da52f1f5c7ce0e069707a3235613e77263b9da5dcff0737f2d09ea1f5000001e72d286979ee6cb1b7e65dfddfb2e384100b8d148e7758de42e4168b71792c6040bad59736020000f41cdd4b7ec41847443fa36bf8dde0009d7ecebc0141403af6b2ad6f7630f81eaaff485073c0fe4f337102d1ecf0a48ed9bcfbd4a4bbeb5d7ae26f7dd5e0e04527b313187dfe6a6a0cd7f85fd0ce431f609acce1d34aff2321039f07df7861c216de3b78c647b77f8b01404b400a437302b651cdf206ec1af626ac".HexToBytes();
            var tx = (RegisterTransaction)_deserializer.Deserialize<Transaction>(data);

            tx.UpdateHash(_serializer, _crypto);

            Assert.AreEqual(tx.Hash.ToString(true), "0x439af8273fbe25fec2f5f2066679e82314fe0776d52a8c1c87e863bd831ced7d");
            Assert.AreEqual(tx.Attributes.Length, 0);

            Assert.AreEqual(tx.Admin.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Amount.ToString(), "1024");
            Assert.AreEqual(tx.AssetType, AssetType.Token);
            Assert.AreEqual(tx.Name, "[{\"lang\":\"zh-CN\",\"name\":\"Hello AntShares Mainnet\"}]");
            Assert.AreEqual(tx.Precision, 8);
            Assert.AreEqual(tx.Owner.ToString(true), "0x039f07df7861c216de3b78c647b77f8b01404b400a437302b651cdf206ec1af626");

            Assert.AreEqual(tx.Scripts.Length, 1);
            Assert.AreEqual(tx.Scripts[0].Hash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Scripts[0].InvocationScript.ToHexString(true), "0x403af6b2ad6f7630f81eaaff485073c0fe4f337102d1ecf0a48ed9bcfbd4a4bbeb5d7ae26f7dd5e0e04527b313187dfe6a6a0cd7f85fd0ce431f609acce1d34aff");
            Assert.AreEqual(tx.Scripts[0].VerificationScript.ToHexString(true), "0x21039f07df7861c216de3b78c647b77f8b01404b400a437302b651cdf206ec1af626ac");

            Assert.AreEqual(tx.Inputs.Length, 1);
            Assert.AreEqual(tx.Inputs[0].PrevHash.ToString(true), "0xf5a19ed0f23707ffdca59d3b26773e6135327a7069e0e07c5c1f2fa51d76bab3");
            Assert.AreEqual(tx.Inputs[0].PrevIndex, 0);

            Assert.AreEqual(tx.Outputs.Length, 1);

            Assert.AreEqual(tx.Outputs[0].AssetId.ToString(true), "0x602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7");
            Assert.AreEqual(tx.Outputs[0].ScriptHash.ToString(true), "0xbcce7e9d00e0ddf86ba33f444718c47e4bdd1cf4");
            Assert.AreEqual(tx.Outputs[0].Value.ToString(), "24334.98856");

            Assert.AreEqual(tx.Version, 0);
        }

        /*
        [TestMethod]
        public void SerializeDeserialize_StateTransaction()
        {
            // Mainnet Block= / Tx=

            var data = "".HexToBytes();
            var tx = (StateTransaction)_deserializer.Deserialize<Transaction>(data);

            tx.UpdateHash(_serializer, _crypto);

        }

        [TestMethod]
        public void SerializeDeserialize_PublishTransaction()
        {
            // Mainnet Block= / Tx=

            var data = "".HexToBytes();
            var tx = (PublishTransaction)_deserializer.Deserialize<Transaction>(data);

            tx.UpdateHash(_serializer, _crypto);

        }

        [TestMethod]
        public void SerializeDeserialize_EnrollmentTransaction()
        {
            // Mainnet Block= / Tx=

            var data = "".HexToBytes();
            var tx = (EnrollmentTransaction)_deserializer.Deserialize<Transaction>(data);

            tx.UpdateHash(_serializer, _crypto);

        }
        */
    }
}