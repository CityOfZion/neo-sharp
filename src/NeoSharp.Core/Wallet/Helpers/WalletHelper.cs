using System;
using System.Linq;
using System.Security;
using System.Text;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.SmartContract;
using NeoSharp.Cryptography;
using NeoSharp.Types;

namespace NeoSharp.Core.Wallet.Helpers
{
    public class WalletHelper
    {
        /// <summary>
        /// Decrypts a NEP-2 into a private key.
        /// https://github.com/neo-project/proposals/blob/master/nep-2.mediawiki#decryption-steps
        /// //Decryption steps
        /// 1 - Collect encrypted private key and passphrase from user.
        /// 2 - Derive derivedhalf1 and derivedhalf2 by passing the passphrase and addresshash into scrypt function.
        /// 3 - Decrypt encryptedhalf1 and encryptedhalf2 using AES256Decrypt, merge the two parts and XOR the result with derivedhalf1 to form the plaintext private key.
        /// 4 - Convert that plaintext private key into a NEO address.
        /// 5 - Hash the NEO address, and verify that addresshash from the encrypted private key record matches the hash.If not, report that the passphrase entry was incorrect.
        /// </summary>
        /// <returns>The wif.</returns>
        /// <param name="encryptedPrivateKey">Nep2.</param>
        /// <param name="passphrase">Passphrase.</param>
        public byte[] DecryptWif(string encryptedPrivateKey, SecureString passphrase)
        {
            if (string.IsNullOrWhiteSpace(encryptedPrivateKey))
            {
                throw new ArgumentNullException(nameof(encryptedPrivateKey));
            }

            if (passphrase == null || string.IsNullOrWhiteSpace(passphrase.ToString()))
            {
                throw new ArgumentNullException(nameof(passphrase));
            }

            // Data is encoded in Base58
            // https://github.com/neo-project/proposals/blob/master/nep-2.mediawiki#abstract
            var data = Crypto.Default.Base58CheckDecode(encryptedPrivateKey);

            //2 bytes 0x0142 Object Identifier Prefix (see below)
            //1 byte(flagbyte): always be 0xE0
            //4 bytes: SHA256(SHA256(expected_neo_address))[0...3], used both for typo checking and as salt
            //16 bytes: An AES - encrypted key material record(encryptedhalf1)
            //16 bytes: An AES - encrypted key material record(encryptedhalf2)
            if (data.Length != 39)
            {
                throw new FormatException();
            }

            // Object identifier prefix: 0x0142.
            // These are constant bytes that appear at the beginning of the Base58Check - encoded record, 
            // and their presence causes the resulting string to have a predictable prefix.
            // How the user sees it: 58 characters always starting with '6P'
            // https://github.com/neo-project/proposals/blob/master/nep-2.mediawiki#proposed-specification
            if (data[0] != 0x01 || data[1] != 0x42)
            {
                throw new FormatException();
            }
            // Payload flagbyte, always 0xE0
            if (data[2] != 0xe0)
            {
                throw new FormatException();
            }

            //4 bytes: SHA256(SHA256(expected_neo_address))[0...3], used both for typo checking and as salt
            var addressHash = new byte[4];
            Buffer.BlockCopy(data, 3, addressHash, 0, 4);

            //Passphrase encoded in UTF - 8 and normalized using Unicode Normalization Form C(NFC). 
            var passphraseUtf8String = passphrase.ToByteArray();

            //Derive derivedhalf1 and derivedhalf2 by passing the passphrase and addresshash into scrypt function.
            var derivedKey = Crypto.Default.SCrypt(passphraseUtf8String, addressHash, ScryptParameters.Default.N, ScryptParameters.Default.R, ScryptParameters.Default.P, 64);

            //Decrypt encryptedhalf1 and encryptedhalf2 using AES256Decrypt, 
            var derivedhalf1 = derivedKey.Take(32).ToArray();
            var derivedhalf2 = derivedKey.Skip(32).ToArray();

            var encryptedkey = new byte[32];
            Buffer.BlockCopy(data, 7, encryptedkey, 0, 32);

            //merge the two parts and XOR the result with derivedhalf1 to form the plaintext private key.
            var privateKey = Crypto.Default.AesDecrypt(encryptedkey, derivedhalf2).XOR(derivedhalf1);

            //Integrity check. Its necessary to rebuild the contract to get the address
            var address = privateKeyToAddress(privateKey);

            var addressBytes = Encoding.ASCII.GetBytes(address);
            var check = Crypto.Default.Sha256(Crypto.Default.Sha256(addressBytes)).Take(4);

            if (!check.SequenceEqual(addressHash))
            {
                throw new FormatException();
            }

            return privateKey;
        }

        /// <summary>
        /// Creates a protected private key following NEP-2
        /// https://github.com/neo-project/proposals/blob/master/nep-2.mediawiki#encryption-steps
        /// 1 - Compute the NEO address (ASCII), and take the first four bytes of SHA256(SHA256()) of it. Let's call this "addresshash".
        /// 2 - Derive a key from the passphrase using scrypt
        ///     Parameters: passphrase is the passphrase itself encoded in UTF-8 and normalized using Unicode Normalization Form C(NFC). Salt is the addresshash from the earlier step, n = 16384, r = 8, p = 8, length = 64
        ///     Let's split the resulting 64 bytes in half, and call them derivedhalf1 and derivedhalf2.
        /// 3 - Do AES256Encrypt(block = privkey[0...15] xor derivedhalf1[0...15], key = derivedhalf2), call the 16-byte result encryptedhalf1
        /// 4 - Do AES256Encrypt(block = privkey[16...31] xor derivedhalf1[16...31], key = derivedhalf2), call the 16-byte result encryptedhalf2
        /// The encrypted private key is the Base58Check-encoded concatenation of the following, which totals 39 bytes without Base58 checksum:
        /// 0x01 0x42 + flagbyte + addresshash + encryptedhalf1 + encryptedhalf2
        /// </summary>
        /// <returns>The nep2 from public key and private key.</returns>
        /// <param name="privateKey">Private key.</param>
        public string EncryptWif(byte[] privateKey, SecureString passphrase)
        {

            //1 - Compute the NEO address(ASCII), and take the first four bytes of SHA256(SHA256()) of it. 
            // Let's call this "addresshash".
            var address = privateKeyToAddress(privateKey);
            var addressBytes = Encoding.ASCII.GetBytes(address);
            var addressHash = Crypto.Default.Sha256(Crypto.Default.Sha256(addressBytes)).Take(4).ToArray();

            //Passphrase encoded in UTF - 8 and normalized using Unicode Normalization Form C(NFC). 
            var passphraseUtf8String = passphrase.ToByteArray();

            /// 2 - Derive a key from the passphrase using scrypt
            ///     Parameters: passphrase is the passphrase itself encoded in UTF-8 and normalized using Unicode Normalization Form C(NFC). 
            ///     Salt is the addresshash from the earlier step, n = 16384, r = 8, p = 8, length = 64
            ///     Let's split the resulting 64 bytes in half, and call them derivedhalf1 and derivedhalf2.
            var derivedkey = Crypto.Default.SCrypt(passphraseUtf8String, addressHash, ScryptParameters.Default.N, ScryptParameters.Default.R, ScryptParameters.Default.P, 64);
            var derivedhalf1 = derivedkey.Take(32).ToArray();
            var derivedhalf2 = derivedkey.Skip(32).ToArray();


            /// 3 & 4  - Do AES256Encrypt(block = privkey[0...15] xor derivedhalf1[0...15], key = derivedhalf2), call the 16-byte result encryptedhalf1
            var encryptedKey = Crypto.Default.AesEncrypt(privateKey.XOR(derivedhalf1), derivedhalf2);

            /// The encrypted private key is the Base58Check-encoded concatenation of the following, which totals 39 bytes without Base58 checksum:
            /// 0x01 0x42 + flagbyte + addresshash + encryptedhalf1 + encryptedhalf2
            var buffer = new byte[39];
            buffer[0] = 0x01;
            buffer[1] = 0x42;
            buffer[2] = 0xe0;

            //Address Hash
            Buffer.BlockCopy(addressHash, 0, buffer, 3, addressHash.Length);
            //Encrypted wif
            Buffer.BlockCopy(encryptedKey, 0, buffer, 7, encryptedKey.Length);

            return Crypto.Default.Base58CheckEncode(buffer);
        }

        /// <summary>
        /// Script hash from public key.
        /// </summary>
        /// <returns>The script hash.</returns>
        /// <param name="publicKey">Public key.</param>
        public UInt160 ScriptHashFromPublicKey(ECPoint publicKey)
        {
            return ContractFactory.CreateSinglePublicKeyRedeemContract(publicKey).ScriptHash;
        }

        /// <summary>
        /// Converts a byte array into wif string
        /// </summary>
        /// <returns>Wif.</returns>
        /// <param name="privateKey">Private Key</param>
        public string PrivateKeyToWif(byte[] privateKey)
        {
            byte[] data = new byte[34];
            data[0] = 0x80;
            Buffer.BlockCopy(privateKey, 0, data, 1, 32);
            data[33] = 0x01;
            string wif = Crypto.Default.Base58CheckEncode(data);
            Array.Clear(data, 0, data.Length);
            return wif;
        }

        /// <summary>
        /// Gets the private key from wif.
        /// </summary>
        /// <returns>The private key from wif.</returns>
        /// <param name="wif">Wif.</param>
        public byte[] GetPrivateKeyFromWIF(string wif)
        {
            var internalWif = wif ?? throw new ArgumentNullException(nameof(wif));

            var privateKeyByteArray = Crypto.Default.Base58CheckDecode(internalWif);

            if (privateKeyByteArray.IsValidPrivateKey())
            {
                var privateKey = new byte[32];
                Buffer.BlockCopy(privateKeyByteArray, 1, privateKey, 0, privateKey.Length);
                Array.Clear(privateKeyByteArray, 0, privateKeyByteArray.Length);
                return privateKey;
            }
            else
            {
                throw new FormatException();
            }
        }

        #region Private Methods

        /// <summary>
        /// Privates the key to address.
        /// </summary>
        /// <returns>The key to address.</returns>
        /// <param name="privateKey">Private key.</param>
        private string privateKeyToAddress(byte[] privateKey)
        {
            var pubKeyInBytes = Crypto.Default.ComputePublicKey(privateKey, true);
            var pubkey = new ECPoint(pubKeyInBytes);
            var accountContract = ContractFactory.CreateSinglePublicKeyRedeemContract(pubkey);
            return accountContract.ScriptHash.ToAddress();
        }

        #endregion
    }
}