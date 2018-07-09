using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using NeoSharp.Core.Wallet.Helpers;
using NeoSharp.Core.Wallet.NEP6;
using NeoSharp.Core.Wallet.Wrappers;
using NeoSharp.TestHelpers;



namespace NeoSharp.Core.Wallet.Test
{
    [TestClass]
    public class NEP6WalletTest : TestBase
    {
        Nep6WalletManager _walletManager;
        WalletHelper _walletHelper;
        SecureString _defaultPassword;


        [TestInitialize]
        public void Init()
        {
            //A static file was causing tests to fail, even if I deleted it on CleanUp
            Random random = new Random();
            String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String fileName = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

            //TODO: Mock
            _walletManager = new Nep6WalletManager(new FileWrapper(), new JsonConverterWrapper());
            //TODO: Remove & Mock
            _walletManager.CreateWallet(fileName);

            _walletHelper = new WalletHelper();
            _defaultPassword = new SecureString();
            _defaultPassword.AppendChar('1');
            _defaultPassword.AppendChar('2');
            _defaultPassword.AppendChar('3');
            _defaultPassword.AppendChar('4');
            _defaultPassword.AppendChar('5');
            _defaultPassword.AppendChar('6');
            _defaultPassword.AppendChar('7');
            _defaultPassword.AppendChar('8');
            _defaultPassword.AppendChar('9');
            _defaultPassword.AppendChar('0');

        }


        [TestMethod]
        public void TestGetAccount()
        {
            // Act

            IWalletAccount walletAccount = _walletManager.CreateAccount(_defaultPassword);
            IWalletAccount walletAccountFromWallet = _walletManager.GetAccount(walletAccount.Contract.ScriptHash);

            Assert.IsNotNull(walletAccount);
        }

        [TestMethod]
        public void TestGetAccountPublicKey()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.ImportWif("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a", _defaultPassword);
            byte[] privateKey = GetPrivateKeyFromWIF("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a");
            ECPoint publicKey = new ECPoint(ICrypto.Default.ComputePublicKey(privateKey, true));

            IWalletAccount walletAccount2 = _walletManager.GetAccount(publicKey);

            Assert.IsNotNull(walletAccount);
            //TODO: Improve
            //Assert.IsFalse(String.IsNullOrWhiteSpace(walletAccount.Address));
        }

        [TestMethod]
        public void TestContains()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.CreateAccount(_defaultPassword);
            bool contains = _walletManager.Contains(walletAccount.Contract.ScriptHash);

            Assert.IsTrue(contains);
        }

        [TestMethod]
        public void TestContainsFalse()
        {
            bool contains = _walletManager.Contains(UInt160.Zero);
            Assert.IsFalse(contains);
        }

        [TestMethod]
        public void TestDeleteAccount()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.CreateAccount(_defaultPassword);

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);

            _walletManager.DeleteAccount(walletAccount.Contract.ScriptHash);

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 0);
        }

        [TestMethod]
        public void TestDeleteAccountFalse()
        {
            // Act
            IWalletAccount walletAccount1 = _walletManager.CreateAccount(_defaultPassword);

            IWalletAccount walletAccount2 = _walletManager.CreateAccount(_defaultPassword);

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 2);

            _walletManager.DeleteAccount(walletAccount1.Contract.ScriptHash);

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.CreateAccount(_defaultPassword);

            // Asset
            Assert.IsNotNull(walletAccount);

            Assert.IsNotNull(walletAccount.Contract.ScriptHash);

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestImportScriptHash()
        {
            // Act
            IWalletAccount walletAccount1 = _walletManager.CreateAccount(_defaultPassword);

            IWalletAccount walletAccount2 = _walletManager.ImportScriptHash(walletAccount1.Contract.ScriptHash);

            // Asset
            Assert.IsNotNull(walletAccount2);

            //TODO: Check & improve
            //Assert.IsFalse(String.IsNullOrEmpty(walletAccount2.Address));

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestImportPrivateKey()
        {
            byte[] privateKey = GetPrivateKeyFromWIF("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a");
            // Act
            IWalletAccount walletAccount = _walletManager.ImportPrivateKey(privateKey, _defaultPassword);

            // Asset
            Assert.IsNotNull(walletAccount);

            String address = walletAccount.Contract.ScriptHash.ToAddress();
            Assert.IsTrue(address.Equals("AdYJeaHepN3jmdUduBLWPESqwQ9QYQCFi7"));

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestImportNEP6AndPassphrase()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.ImportEncryptedWif("6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv", _defaultPassword);


            // Asset
            Assert.IsNotNull(walletAccount);

            String address = walletAccount.Contract.ScriptHash.ToAddress();
            Assert.IsTrue(address.Equals("AdYJeaHepN3jmdUduBLWPESqwQ9QYQCFi7"));

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestImportWif()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.ImportWif("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a", _defaultPassword);

            // Asset
            Assert.IsNotNull(walletAccount);

            String address = walletAccount.Contract.ScriptHash.ToAddress();
            Assert.IsTrue(address.Equals("AdYJeaHepN3jmdUduBLWPESqwQ9QYQCFi7"));

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestVerifyPasswordFalse()
        {
            // Act
            SecureString fakeString = new SecureString();
            fakeString.AppendChar('1');
            bool result = _walletManager.VerifyPassword(new NEP6Account() { Key = "6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv" }, fakeString);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestVerifyPassword()
        {
            // Act
            bool result = _walletManager.VerifyPassword(new NEP6Account() { Key = "6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv" }, _defaultPassword);

            Assert.IsTrue(result);
        }

        #region ExpectedException

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestImportScriptHashEmpty()
        {
            // Act
            IWalletAccount walletAccount1 = new NEP6Account()
            {
                Contract =  new Contract () { 
                    Code = new Code { 
                        ScriptHash = UInt160.Zero
                    }
                }
            };

            _walletManager.ImportScriptHash(walletAccount1.Contract.ScriptHash);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeleteAccountNull()
        {
            // Act
            _walletManager.DeleteAccount(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestVerifyPasswordAccountNull()
        {
            // Act
            _walletManager.VerifyPassword(null, _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestContainsNull()
        {
            // Act
            _walletManager.Contains(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestVerifyPasswordNep2KeyNull()
        {
            // Act
            _walletManager.VerifyPassword(new NEP6Account() { Key = null }, _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestVerifyPasswordNep2KeyEmpty()
        {
            // Act
            _walletManager.VerifyPassword(new NEP6Account() { Key = String.Empty }, _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestVerifyPasswordPasswordNull()
        {
            // Act
            _walletManager.VerifyPassword(new NEP6Account() { Key = "6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv" }, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestVerifyPasswordPasswordEmpty()
        {
            // Act
            _walletManager.VerifyPassword(new NEP6Account() { Key = "6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv" }, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestImportWifNull()
        {
            // Act
            _walletManager.ImportPrivateKey(new byte[] { }, _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportWifEmpty()
        {
            // Act
            _walletManager.ImportWif(String.Empty, _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharaseNEP6Null()
        {
            // Act
            _walletManager.ImportEncryptedWif(null, _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharaseNEP6Empty()
        {
            // Act
            _walletManager.ImportEncryptedWif(String.Empty, _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharasePassphareNull()
        {
            // Act
            _walletManager.ImportEncryptedWif("6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharasePassphareEmpty()
        {
            // Act
            _walletManager.ImportEncryptedWif("6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv", null);
        }

        #endregion

        private byte[] GetPrivateKeyFromWIF(string wif)
        {
            if (wif == null)
            {
                throw new ArgumentNullException();
            }

            byte[] data = ICrypto.Default.Base58CheckDecode(wif);

            if (data.Length != 34 || data[0] != 0x80 || data[33] != 0x01)
            {
                throw new FormatException();
            }

            byte[] privateKey = new byte[32];
            Buffer.BlockCopy(data, 1, privateKey, 0, privateKey.Length);
            Array.Clear(data, 0, data.Length);
            return privateKey;
        }


    }
}
