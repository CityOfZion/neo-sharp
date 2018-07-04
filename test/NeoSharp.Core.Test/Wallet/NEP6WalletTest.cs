using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using NeoSharp.Core.Wallet.NEP6;
using NeoSharp.TestHelpers;


namespace NeoSharp.Core.Wallet.Test
{
    [TestClass]
    public class NEP6WalletTest : TestBase
    {
        Nep6WalletManager _walletManager;
        ICrypto _crypto;
        FileInfo TestWalletFile;

        [TestInitialize]
        public void Init()
        {
            //A static file was causing tests to fail, even if I deleted it on CleanUp
            Random random = new Random();
            String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String fileName = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
            TestWalletFile = new FileInfo(fileName);

            _crypto = AutoMockContainer.Create<BouncyCastleCrypto>();
            _walletManager = new Nep6WalletManager(_crypto);
            _walletManager.CreateWallet(TestWalletFile);
        }

        [TestCleanup]
        public void CleanUp()
        {
            TestWalletFile.Delete();
        }

        [TestMethod]
        public void TestGetAccount()
        {
            // Act

            IWalletAccount walletAccount = _walletManager.CreateAccount();
            IWalletAccount walletAccountFromWallet = _walletManager.GetAccount(walletAccount.ScriptHash);

            Assert.IsNotNull(walletAccount);
            Assert.IsFalse(String.IsNullOrWhiteSpace(walletAccount.Address));
        }

        [TestMethod]
        public void TestGetAccountPublicKey()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.Import("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a");
            byte[] privateKey = GetPrivateKeyFromWIF("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a");
            ECPoint publicKey = new ECPoint(_crypto.ComputePublicKey(privateKey, true));

            IWalletAccount walletAccount2 = _walletManager.GetAccount(publicKey);

            Assert.IsNotNull(walletAccount);
            Assert.IsFalse(String.IsNullOrWhiteSpace(walletAccount.Address));
        }

        [TestMethod]
        public void TestContains()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.CreateAccount();
            bool contains = _walletManager.Contains(walletAccount.ScriptHash);

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
            IWalletAccount walletAccount = _walletManager.CreateAccount();

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);

            _walletManager.DeleteAccount(walletAccount.ScriptHash);

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 0);
        }

        [TestMethod]
        public void TestDeleteAccountFalse()
        {
            // Act
            IWalletAccount walletAccount1 = _walletManager.CreateAccount();

            IWalletAccount walletAccount2 = _walletManager.CreateAccount();

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 2);

            _walletManager.DeleteAccount(walletAccount1.ScriptHash);

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.CreateAccount();

            // Asset
            Assert.IsNotNull(walletAccount);

            Assert.IsFalse(String.IsNullOrEmpty(walletAccount.Address));

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestImportScriptHash()
        {
            // Act
            IWalletAccount walletAccount1 = _walletManager.CreateAccount();

            IWalletAccount walletAccount2 = _walletManager.Import(walletAccount1.ScriptHash);

            // Asset
            Assert.IsNotNull(walletAccount2);

            Assert.IsFalse(String.IsNullOrEmpty(walletAccount2.Address));

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestImportPrivateKey()
        {
            byte[] privateKey = GetPrivateKeyFromWIF("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a");
            // Act
            IWalletAccount walletAccount = _walletManager.Import(privateKey);

            // Asset
            Assert.IsNotNull(walletAccount);

            Assert.IsTrue(walletAccount.Address.Equals("AdYJeaHepN3jmdUduBLWPESqwQ9QYQCFi7"));

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestImportNEP6AndPasspharase()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.Import("6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv", "1234567890");

            // Asset
            Assert.IsNotNull(walletAccount);

            Assert.IsTrue(walletAccount.Address.Equals("AdYJeaHepN3jmdUduBLWPESqwQ9QYQCFi7"));

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestImportWif()
        {
            // Act
            IWalletAccount walletAccount = _walletManager.Import("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a");

            // Asset
            Assert.IsNotNull(walletAccount);

            Assert.IsTrue(walletAccount.Address.Equals("AdYJeaHepN3jmdUduBLWPESqwQ9QYQCFi7"));

            Assert.IsTrue(_walletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        public void TestVerifyPasswordFalse()
        {
            // Act
            bool result = _walletManager.VerifyPassword(new NEP6Account(_crypto) { Key = "6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv" }, "1111111");

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void TestVerifyPassword()
        {
            // Act
            bool result = _walletManager.VerifyPassword(new NEP6Account(_crypto) { Key = "6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv" }, "1234567890");

            Assert.IsTrue(result);
        }

        #region ExpectedException

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestImportScriptHashEmpty()
        {
            // Act
            IWalletAccount walletAccount1 = new NEP6Account(_crypto)
            {
                ScriptHash = new UInt160()
            };

            _walletManager.Import(walletAccount1.ScriptHash);
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
            _walletManager.VerifyPassword(null, "1234567890");
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
            _walletManager.VerifyPassword(new NEP6Account(_crypto) { Key = null }, "1234567890");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestVerifyPasswordNep2KeyEmpty()
        {
            // Act
            _walletManager.VerifyPassword(new NEP6Account(_crypto) { Key = String.Empty }, "1234567890");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestVerifyPasswordPasswordNull()
        {
            // Act
            _walletManager.VerifyPassword(new NEP6Account(_crypto) { Key = "6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv" }, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestVerifyPasswordPasswordEmpty()
        {
            // Act
            _walletManager.VerifyPassword(new NEP6Account(_crypto) { Key = "6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv" }, String.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestImportWifNull()
        {
            // Act
            _walletManager.Import(new byte[] { });
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportWifEmpty()
        {
            // Act
            _walletManager.Import(String.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharaseNEP6Null()
        {
            // Act
            _walletManager.Import(null, "1234567890");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharaseNEP6Empty()
        {
            // Act
            _walletManager.Import(String.Empty, "1234567890");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharasePassphareNull()
        {
            // Act
            _walletManager.Import("6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharasePassphareEmpty()
        {
            // Act
            _walletManager.Import("6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv", String.Empty);
        }

        #endregion

        private byte[] GetPrivateKeyFromWIF(string wif)
        {
            if (wif == null)
            {
                throw new ArgumentNullException();
            }

            byte[] data = _crypto.Base58CheckDecode(wif);

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
