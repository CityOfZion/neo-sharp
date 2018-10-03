using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Cryptography;
using NeoSharp.TestHelpers;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Test.Cryptography
{
    [TestClass]
    public class UtCrypto : TestBase
    {
        private Crypto _bccrypto;
        private Crypto _nativecrypto;
        private byte[] _data;

        [TestInitialize]
        public void Init()
        {
            _bccrypto = AutoMockContainer.Create<BouncyCastleCrypto>();
            _nativecrypto = AutoMockContainer.Create<NativeCrypto>();
            _data = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };
        }

        [TestMethod]
        public void Initialize()
        {
            // Act
            Crypto.Initialize(_bccrypto);
            Assert.AreEqual(_bccrypto.GetType(), Crypto.Default.GetType());

            Crypto.Initialize(_nativecrypto);
            Assert.AreEqual(_nativecrypto.GetType(), Crypto.Default.GetType());
        }

        [TestMethod]
        public void Sha256()
        {
            // Act
            var sha256Bc = _bccrypto.Sha256(_data);
            var sha256Nv = _nativecrypto.Sha256(_data);

            // Assert
            CollectionAssert.AreEqual(sha256Bc, sha256Nv);
            Assert.AreEqual(sha256Bc.ToHexString(), "be45cb2605bf36bebde684841a28f0fd43c69850a3dce5fedba69928ee3a8991");
        }

        [TestMethod]
        public void RIPEMD160()
        {
            // Act
            var ripemd160 = _bccrypto.RIPEMD160(_data);

            // Assert
            Assert.AreEqual(ripemd160.ToHexString(), "696c7528a3545e25bec296e0d39b5f898bec97f7");
        }

        [TestMethod]
        public void Hash256()
        {
            // Act
            var hash256 = _bccrypto.Hash256(_data);

            // Assert
            Assert.AreEqual(hash256.ToHexString(), "fc793c641b354b10b9a264ad4f541f6efe8445a0d05fe39336a126252b166e8b");
        }

        [TestMethod]
        public void Hash160()
        {
            // Act
            var hash160 = _bccrypto.Hash160(_data);

            // Assert
            Assert.AreEqual(hash160.ToHexString(), "7a91e1b6ef1be3631b154ac8763a017eb03dc1b0");
        }

        [TestMethod]
        public void Murmur3()
        {
            // Act
            var murmur3 = _bccrypto.Murmur3(_data, 1);

            // Assert
            Assert.AreEqual(murmur3.ToHexString(), "5376a7bb");
        }

        [TestMethod]
        public void Murmur32()
        {
            // Act
            var murmur32 = _bccrypto.Murmur32(_data, 1);

            // Assert
            Assert.AreEqual(murmur32, 3148314195);
        }

        [TestMethod]
        public void VerifySignature()
        {
            // Arrange
            var message1 = "00000000bf4421c88776c53b43ce1dc45463bfd2028e322fdfb60064be150ed3e36125d418f98ec3ed2c2d1c9427385e7b85d0d1a366e29c4e399693a59718380f8bbad6d6d90358010000004490d0bb7170726c59e75d652b5d3827bf04c165bbe9ef95cca4bf55".HexToBytes();
            var signature1 = "4e0ebd369e81093866fe29406dbf6b402c003774541799d08bf9bb0fc6070ec0f6bad908ab95f05fa64e682b485800b3c12102a8596e6c715ec76f4564d5eff3".HexToBytes();
            var pubkey1 = "ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba5543672835e89a5c1f821d773214881e84618770508ce1ddfd488ae377addf7ca38".HexToBytes();
            var message2 = "00000000ea5029691bd94d9667cb32bf136cbba38cf9eb5978bd1d0bf825a3f8a80be6af157aee574e343ff867f3c470ffeecd77312bed61195ba8f1c6588fd275257f60ef6b0458d6070000a36a49f800ef916159e75d652b5d3827bf04c165bbe9ef95cca4bf55".HexToBytes();
            var signature2 = "95083c5c98cdacdaf57af61104b68940cd0f7cae59b907ddea7f77ae1c4884348321ab62e65eabd82876e2e5f58f822538633521307be831a260ecab2cc5d16c".HexToBytes();
            var pubkey2 = "03b8d9d5771d8f513aa0869b9cc8d50986403b78c6da36890638c3d46a5adce04a".HexToBytes();
            var message3 = "00000000ea5029691bd94d9667cb32bf136cbba38cf9eb5978bd1d0bf825a3f8a80be6af157aee574e343ff867f3c470ffeecd77312bed61195ba8f1c6588fd275257f60ef6b0458d6070000a36a49f800ef916159e75d652b5d3827bf04c165bbe9ef95cca4bf55".HexToBytes();
            var signature3 = "95083c5c98cdacdaf57af61104b68940cd0f7cae59b907ddea7f77ae1c4884348321ab62e65eabd82876e2e5f58f822538633521307be831a260ecab2cc5d16c".HexToBytes();
            var pubkey3 = "ca0e27697b9c248f6f16e085fd0061e26f44da85b58ee835c110caa5ec3ba5543672835e89a5c1f821d773214881e84618770508ce1ddfd488ae377addf7ca38".HexToBytes();

            // Act
            var verify1 = _bccrypto.VerifySignature(message1, signature1, pubkey1);
            var verify2 = _bccrypto.VerifySignature(message2, signature2, pubkey2);
            var verifybad = _bccrypto.VerifySignature(message3, signature3, pubkey3);

            // Assert
            Assert.IsTrue(verify1);
            Assert.IsTrue(verify2);
            Assert.IsFalse(verifybad);
        }

        [TestMethod]
        public void Sign()
        {
            // Arrange
            var privkey = "d422260f1d97788bd0ee4d089e57a9bd20356a4013492cafd4d0dcf9efc68968".HexToBytes();
            var pubkey = "0438356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6fc88c3095b1b688d3d027477dfad0deb1ab94cb08db2de5abb79c1482aa1ea2fc".HexToBytes();
            var message = "00000000bf4421c88776c53b43ce1dc45463bfd2028e322fdfb60064be150ed3e36125d418f98ec3ed2c2d1c9427385e7b85d0d1a366e29c4e399693a59718380f8bbad6d6d90358010000004490d0bb7170726c59e75d652b5d3827bf04c165bbe9ef95cca4bf55".HexToBytes();
            var signaturebad = new byte[64];

            for (int i = 0; i < 50; i++)
            {
                // Act
                var signature = _bccrypto.Sign(message, privkey);
                Array.Copy(signature, signaturebad, 64);
                signaturebad[0] = 0x11;
                signaturebad[10] = 0xFF;
                signaturebad[63] = 0x00;
                var verify = _bccrypto.VerifySignature(message, signature, pubkey);
                var verifybad = _bccrypto.VerifySignature(message, signaturebad, pubkey);

                // Assert
                Assert.IsTrue(verify);
                Assert.IsFalse(verifybad);
            }
        }

        [TestMethod]
        public void PubKey_Generation()
        {
            // Arrange
            var privkey = "d422260f1d97788bd0ee4d089e57a9bd20356a4013492cafd4d0dcf9efc68968".HexToBytes();
            var expectedPubkeyComp = "0238356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6f".HexToBytes();
            var expectedPubkeyUncomp = "0438356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6fc88c3095b1b688d3d027477dfad0deb1ab94cb08db2de5abb79c1482aa1ea2fc".HexToBytes();

            // Act
            var pubkeyCompressed = _bccrypto.ComputePublicKey(privkey, true);
            var pubkeyUncompressed = _bccrypto.ComputePublicKey(privkey, false);

            // Assert
            CollectionAssert.AreEqual(expectedPubkeyComp, pubkeyCompressed);
            CollectionAssert.AreEqual(expectedPubkeyUncomp, pubkeyUncompressed);
        }

        [TestMethod]
        public void Decode_PublicKey()
        {
            // Arrange
            var pubkeyPart = "38356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6fc88c3095b1b688d3d027477dfad0deb1ab94cb08db2de5abb79c1482aa1ea2fc".HexToBytes();
            var pubkeyUncomp = "0438356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6fc88c3095b1b688d3d027477dfad0deb1ab94cb08db2de5abb79c1482aa1ea2fc".HexToBytes();
            var pubkeyComp = "0238356c74a1ab4d40df857b790e4232180e2f99f5c78468c150d0903a3e5d2b6f".HexToBytes();

            // Act
            var compressed = _bccrypto.DecodePublicKey(pubkeyUncomp, true, out var x1, out var y1);
            var uncompressed = _bccrypto.DecodePublicKey(pubkeyComp, false, out var x2, out var y2);
            var compressed2 = _bccrypto.DecodePublicKey(pubkeyPart, true, out var x3, out var y3);

            // Assert
            Assert.AreEqual(x1, x2);
            Assert.AreEqual(y1, y2);
            Assert.AreEqual(x3, x1);
            Assert.AreEqual(x1.ToString(), "25423910948081187308645163652542039167507182320027680753707589392465049496431");
            Assert.AreEqual(y2.ToString(), "90710263625294316789152749637780639904397777988680875310398951746483276129020");
            CollectionAssert.AreEqual(pubkeyComp, compressed);
            CollectionAssert.AreEqual(pubkeyComp, compressed2);
            CollectionAssert.AreEqual(pubkeyUncomp, uncompressed);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Decode_PublicKey_Exception()
        {
            // Act
            _bccrypto.DecodePublicKey(null, true, out _, out _);
        }

        [TestMethod]
        public void Aes_CBC_Encrypt_Decrypt()
        {
            // Arrange
            byte[] key = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                           0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F};
            byte[] iv = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F };

            // Act
            var aesBc = _bccrypto.AesEncrypt(_data, key, iv);
            var aesNv = _nativecrypto.AesEncrypt(_data, key, iv);
            var dataBc = _bccrypto.AesDecrypt(aesNv, key, iv);
            var dataNv = _nativecrypto.AesDecrypt(aesBc, key, iv);

            // Assert
            CollectionAssert.AreEqual(dataBc, dataNv);
            Assert.AreEqual(aesBc.ToHexString(), "7c6c258ccc6a400efacc631452a75a25");
        }

        [TestMethod]
        public void Aes_ECB_Encrypt_Decrypt()
        {
            // Arrange
            byte[] key = { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
                           0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F};

            // Act
            var aesBc = _bccrypto.AesEncrypt(_data, key);
            var aesNv = _nativecrypto.AesEncrypt(_data, key);
            var dataBc = _bccrypto.AesDecrypt(aesNv, key);
            var dataNv = _nativecrypto.AesDecrypt(aesBc, key);

            // Assert
            CollectionAssert.AreEqual(dataBc, dataNv);
            Assert.AreEqual(aesBc.ToHexString(), "737d7012de2b424f76877a9f0255cc22");
        }

        [TestMethod]
        public void SCrypt()
        {
            // Act
            var resut = _bccrypto.SCrypt(_data, _data, 16384, 8, 1, 16);

            // Assert
            Assert.AreEqual(resut.ToHexString(), "99568a5d8855750bb47a9fce6b0eacc6");
        }

        [TestMethod]
        public void Base58_Encode()
        {
            // Arrange
            var test = "Base58 Unit Test Encode";

            // Act
            var result = _bccrypto.Base58Encode(Encoding.ASCII.GetBytes(test));

            // Assert
            Assert.AreEqual(result, "2NXDA7BcYgm9Sfmth7KzYhxPrNq8Gnj6");
        }

        [TestMethod]
        public void Base58_Decode()
        {
            // Arrange
            var test = "2NXDA7BcYgm9Sfmth7KzYhxPqrw4Qpuz";

            // Act
            var decoding = _bccrypto.Base58Decode(test);
            var result = Encoding.ASCII.GetString(decoding);

            // Assert
            Assert.AreEqual(result, "Base58 Unit Test Decode");
        }

        [TestMethod]
        public void Base58_Encode_Decode()
        {
            // Arrange
            var test = RandomString(this.RandomInt(1, 30));

            // Act
            var encodig = _bccrypto.Base58Encode(Encoding.ASCII.GetBytes(test));
            var decoding = _bccrypto.Base58Decode(encodig);
            var result = Encoding.ASCII.GetString(decoding);

            // Assert
            Assert.AreEqual(test, result);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Base58_Decode_Exception()
        {
            // Arrange
            const string test = "QmFzZTU4IFVuaXQgVGVzdCBFeGNlcHRpb24=";

            // Act
            _bccrypto.Base58Decode(test);
        }

        [TestMethod]
        public void Base58Check_Encode()
        {
            // Arrange
            var test = "Base58CheckEncode Unit Test";

            // Act
            var result = _bccrypto.Base58CheckEncode(Encoding.ASCII.GetBytes(test));

            // Assert
            Assert.AreEqual(result, "21i2gvdxyfLDsyXjR12YRnFdJKkKTuwrdF4VmfZcoWW");
        }

        [TestMethod]
        public void Base58Check_Decode()
        {
            // Arrange
            var test = "21i2gvdxyfLDsyXjQAWjxghkFSwvDoSk3bU5AeTczt4";

            // Act
            var decoding = _bccrypto.Base58CheckDecode(test);
            var result = Encoding.ASCII.GetString(decoding);

            // Assert
            Assert.AreEqual(result, "Base58CheckDecode Unit Test");
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void Base58Check_Decode_Exception()
        {
            // Arrange
            const string test = "93i2gvdxyfLDsyXjQAWjxghkFSwvDoSk3bU5AeTcz32";

            // Act
            _bccrypto.Base58CheckDecode(test);
        }

        [TestMethod]
        public void Base58Check_Encode_Decode()
        {
            // Arrange
            var test = RandomString(this.RandomInt(5, 30));

            // Act
            var encodig = _bccrypto.Base58CheckEncode(Encoding.ASCII.GetBytes(test));
            var decoding = _bccrypto.Base58CheckDecode(encodig);
            var result = Encoding.ASCII.GetString(decoding);

            // Assert
            Assert.AreEqual(test, result);
        }

        [TestMethod]
        public void Random_Bytes_Generation()
        {
            // Arrange
            var results = new List<byte[]>();

            for (var tries = 0; tries < 50; ++tries)
            {
                // Act
                var value = _bccrypto.GenerateRandomBytes(10);
                var contains = results.Any(x => x.SequenceEqual(value));
                results.Add(value);

                // Assert
                Assert.AreNotEqual(BitConverter.ToInt32(value, 0), 0);
                Assert.IsFalse(contains);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Random_Bytes_Exception()
        {
            // Arrange
            _bccrypto.GenerateRandomBytes(0);
        }
    }
}