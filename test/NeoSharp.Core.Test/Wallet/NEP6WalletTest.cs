using System;
using System.Linq;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.SmartContract;
using NeoSharp.Core.Wallet.Exceptions;
using NeoSharp.Core.Wallet.Helpers;
using NeoSharp.Core.Wallet.NEP6;
using NeoSharp.Core.Wallet.Wrappers;
using NeoSharp.Cryptography;
using NeoSharp.TestHelpers;
using NeoSharp.Types;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Core.Wallet.Test
{
    [TestClass]
    public class NEP6WalletTest : TestBase
    {
        SecureString _defaultPassword;
        Contract _testContract;

        [TestInitialize]
        public void Init()
        {
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

            var privateKey = Crypto.Default.GenerateRandomBytes(32);
            var publicKey = Crypto.Default.ComputePublicKey(privateKey, true);
            var publicKeyInEcPoint = new ECPoint(publicKey);
            _testContract = ContractFactory.CreateSinglePublicKeyRedeemContract(publicKeyInEcPoint);
        }

        #region Save Wallet

        [TestMethod]
        public void TestSaveWalletSuccess()
        {
            var expectedFileName = "name.json";
            var expectedNep6WalletSerialized = "NEP6WalletSerialized";
            var expetectedWalletVersion = "1.0";
            var expectedWalletName = "name";

            var fileWrapper = AutoMockContainer.GetMock<IFileWrapper>();
            var jsonConverter = AutoMockContainer.GetMock<IJsonConverter>();
            fileWrapper.Setup(x => x.Exists(expectedFileName)).Returns(false);

            jsonConverter
                .Setup(x => x.SerializeObject(It.Is<NEP6Wallet>(wallet => wallet.Name == expectedWalletName && wallet.Version == expetectedWalletVersion)))
                .Returns(expectedNep6WalletSerialized);

            var mockWalletManager = AutoMockContainer.Create<Nep6WalletManager>();

            mockWalletManager.CreateWallet(expectedFileName);
            Assert.IsNotNull(mockWalletManager.Wallet);
            fileWrapper.Verify(x => x.WriteToFile(expectedNep6WalletSerialized, expectedFileName));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSaveWalletFileExists()
        {
            var expectedFileName = "name.json";

            var fileWrapper = AutoMockContainer.GetMock<IFileWrapper>();
            fileWrapper.Setup(x => x.Exists(expectedFileName)).Returns(true);

            var mockWalletManager = AutoMockContainer.Create<Nep6WalletManager>();

            mockWalletManager.CreateWallet(expectedFileName);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSaveWalletJsonEmpty()
        {
            var expectedFileName = "name.json";
            var expetectedWalletVersion = "1.0";
            var expectedWalletName = "name";
            var fileWrapper = AutoMockContainer.GetMock<IFileWrapper>();
            var jsonConverter = AutoMockContainer.GetMock<IJsonConverter>();
            fileWrapper.Setup(x => x.Exists(expectedFileName)).Returns(false);

            jsonConverter
                .Setup(x => x.SerializeObject(It.Is<NEP6Wallet>(wallet => wallet.Name == expectedWalletName && wallet.Version == expetectedWalletVersion)))
                .Returns(String.Empty);

            var mockWalletManager = AutoMockContainer.Create<Nep6WalletManager>();

            mockWalletManager.CreateWallet(expectedFileName);
        }

        #endregion

        #region Contains

        [TestMethod]
        public void TestContains()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            var walletAccount = mockWalletManager.CreateAccount(_defaultPassword);
            var contains = mockWalletManager.Contains(walletAccount.Contract.ScriptHash);

            Assert.IsTrue(contains);
        }

        [TestMethod]
        public void TestContainsFalse()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            var contains = mockWalletManager.Contains(UInt160.Zero);
            Assert.IsFalse(contains);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestContainsNull()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            mockWalletManager.Contains(null);
        }

        #endregion

        #region Create Account

        [TestMethod]
        public void TestCreateAccount()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            var walletAccount = mockWalletManager.CreateAccount(_defaultPassword);

            // Asset
            Assert.IsNotNull(walletAccount);

            Assert.IsNotNull(walletAccount.Contract.ScriptHash);

            Assert.IsTrue(mockWalletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(WalletNotOpenException))]
        public void TestCreateAccountWithoutAnWallet()
        {
            var mockWalletManager = GetAWalletManagerWithoutAnWallet();

            // Act
            mockWalletManager.CreateAccount(_defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(AccountsPasswordMismatchException))]
        public void TestCreateAccountWithDifferentPassword()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            IWalletAccount walletAccount = mockWalletManager.CreateAccount(_defaultPassword);

            var differentPassword = new SecureString();
            differentPassword.AppendChar('x');

            IWalletAccount walletAccount2 = mockWalletManager.CreateAccount(differentPassword);
        }

        [TestMethod]
        public void TestCreateAccountWithSamePassword()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            IWalletAccount walletAccount = mockWalletManager.CreateAccount(_defaultPassword);
            IWalletAccount walletAccount2 = mockWalletManager.CreateAccount(_defaultPassword);
        }

        #endregion

        #region Get Account

        [TestMethod]
        public void TestGetAccount()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();
            // Act

            var walletAccount = mockWalletManager.CreateAccount(_defaultPassword);
            var walletAccountFromWallet = mockWalletManager.GetAccount(walletAccount.Contract.ScriptHash);

            Assert.IsNotNull(walletAccount);
        }

        [TestMethod]
        [ExpectedException(typeof(WalletNotOpenException))]
        public void TestGetAccountPublicKeyWalletIsNotOpened()
        {
            var mockWalletManager = GetAWalletManagerWithoutAnWallet();
            // Act
            IWalletAccount walletAccount = mockWalletManager.ImportWif("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a", _defaultPassword);
            byte[] privateKey = GetPrivateKeyFromWIF("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a");
            ECPoint publicKey = new ECPoint(Crypto.Default.ComputePublicKey(privateKey, true));

            mockWalletManager = new Nep6WalletManager(new FileWrapper(), new JsonConverterWrapper());
            IWalletAccount walletAccount2 = mockWalletManager.GetAccount(publicKey);
        }

        [TestMethod]
        public void TestGetAccountPublicKey()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            IWalletAccount walletAccount = mockWalletManager.ImportWif("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a", _defaultPassword);
            byte[] privateKey = GetPrivateKeyFromWIF("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a");
            ECPoint publicKey = new ECPoint(Crypto.Default.ComputePublicKey(privateKey, true));

            IWalletAccount walletAccount2 = mockWalletManager.GetAccount(publicKey);

            Assert.IsNotNull(walletAccount);
            //TODO #412: Improve address verification testing
            //Assert.IsFalse(String.IsNullOrWhiteSpace(walletAccount.Address));
        }

        #endregion

        #region Delete Account

        [TestMethod]
        public void TestDeleteAccount()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            IWalletAccount walletAccount = mockWalletManager.CreateAndAddAccount(_defaultPassword);

            Assert.IsTrue(mockWalletManager.Wallet.Accounts.Length == 1);

            mockWalletManager.DeleteAccount(walletAccount.Contract.ScriptHash);

            Assert.IsTrue(mockWalletManager.Wallet.Accounts.Length == 0);
        }

        [TestMethod]
        public void TestDeleteAccountFalse()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            IWalletAccount walletAccount1 = mockWalletManager.CreateAndAddAccount(_defaultPassword);

            IWalletAccount walletAccount2 = mockWalletManager.CreateAndAddAccount(_defaultPassword);

            Assert.IsTrue(mockWalletManager.Wallet.Accounts.ToList().Count == 2);

            mockWalletManager.DeleteAccount(walletAccount1.Contract.ScriptHash);

            Assert.IsTrue(mockWalletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(WalletNotOpenException))]
        public void TestDeleteAccountWalletIsNotOpened()
        {
            var mockWalletManager = GetAWalletManagerWithoutAnWallet();
            IWalletAccount walletAccount = mockWalletManager.CreateAndAddAccount(_defaultPassword);

            Assert.IsTrue(mockWalletManager.Wallet.Accounts.ToList().Count == 1);

            mockWalletManager = new Nep6WalletManager(new FileWrapper(), new JsonConverterWrapper());
            mockWalletManager.DeleteAccount(walletAccount.Contract.ScriptHash);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestDeleteAccountNull()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            mockWalletManager.DeleteAccount(null);
        }

        #endregion

        #region Import ScriptHash

        [TestMethod]
        public void TestImportScriptHash()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            NEP6Account walletAccount1 = (NEP6Account)mockWalletManager.CreateAndAddAccount(_defaultPassword);

            NEP6Account walletAccount2 = (NEP6Account)mockWalletManager.ImportScriptHash(walletAccount1.ScriptHash);

            // Assert
            Assert.IsNotNull(walletAccount2);
            Assert.AreEqual(walletAccount1.ScriptHash, walletAccount2.ScriptHash);

            //TODO #412: Check & improve address verification testing
            //Assert.IsFalse(String.IsNullOrEmpty(walletAccount2.Address));

            Assert.IsTrue(mockWalletManager.Wallet.Accounts.Length == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestImportScriptHashEmpty()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act

            var contract = new Contract()
            {
                Code = new Code
                {
                    ScriptHash = UInt160.Zero
                }
            };
            IWalletAccount walletAccount1 = new NEP6Account(contract);

            mockWalletManager.ImportScriptHash(walletAccount1.Contract.ScriptHash);
        }

        [TestMethod]
        [ExpectedException(typeof(WalletNotOpenException))]
        public void TestImportScriptHashWalletIsNotOpened()
        {
            var mockWalletManager = GetAWalletManagerWithoutAnWallet();
            // Act
            NEP6Account walletAccount1 = (NEP6Account)mockWalletManager.CreateAndAddAccount(_defaultPassword);

            mockWalletManager = new Nep6WalletManager(new FileWrapper(), new JsonConverterWrapper());
            NEP6Account walletAccount2 = (NEP6Account)mockWalletManager.ImportScriptHash(walletAccount1.ScriptHash);
        }

        #endregion

        #region Import Private Key

        [TestMethod]
        public void TestImportPrivateKey()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            var privateKey = GetPrivateKeyFromWIF("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a");
            // Act
            var walletAccount = mockWalletManager.ImportPrivateKey(privateKey, _defaultPassword);

            // Asset
            Assert.IsNotNull(walletAccount);

            var address = walletAccount.Contract.ScriptHash.ToAddress();

            Assert.IsTrue(address.Equals("AdYJeaHepN3jmdUduBLWPESqwQ9QYQCFi7"));

            address = address.ToScriptHash().ToAddress();

            Assert.IsTrue(address.Equals("AdYJeaHepN3jmdUduBLWPESqwQ9QYQCFi7"));

            Assert.IsTrue(mockWalletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(WalletNotOpenException))]
        public void TestImportPrivateKeyWalletIsNotOpened()
        {
            var mockWalletManager = GetAWalletManagerWithoutAnWallet();
            var privateKey = GetPrivateKeyFromWIF("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a");
            // Act
            IWalletAccount walletAccount = mockWalletManager.ImportPrivateKey(privateKey, _defaultPassword);
        }

        #endregion

        #region Import NEP6

        [TestMethod]
        public void TestImportNEP6AndPassphrase()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            IWalletAccount walletAccount = mockWalletManager.ImportEncryptedWif("6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv", _defaultPassword);


            // Asset
            Assert.IsNotNull(walletAccount);

            String address = walletAccount.Contract.ScriptHash.ToAddress();
            Assert.IsTrue(address.Equals("AdYJeaHepN3jmdUduBLWPESqwQ9QYQCFi7"));

            Assert.IsTrue(mockWalletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(WalletNotOpenException))]
        public void TestImportNep6AndPassphraseWalletIsNotOpened()
        {
            var mockWalletManager = GetAWalletManagerWithoutAnWallet();

            // Act
            IWalletAccount walletAccount = mockWalletManager.ImportEncryptedWif("6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv", _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharaseNEP6Null()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            mockWalletManager.ImportEncryptedWif(null, _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharaseNEP6Empty()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            mockWalletManager.ImportEncryptedWif(String.Empty, _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharasePassphareNull()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            mockWalletManager.ImportEncryptedWif("6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportNEP6AndPasspharasePassphareEmpty()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            mockWalletManager.ImportEncryptedWif("6PYVwbrWfiyKCFnj4EjjBESUer4hbQ48hPfn8as8ivyS3FTVVmAJomvYuv", null);
        }

        #endregion

        #region Import WIF

        [TestMethod]
        public void TestImportWif()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            IWalletAccount walletAccount = mockWalletManager.ImportWif("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a", _defaultPassword);

            // Asset
            Assert.IsNotNull(walletAccount);

            String address = walletAccount.Contract.ScriptHash.ToAddress();
            Assert.IsTrue(address.Equals("AdYJeaHepN3jmdUduBLWPESqwQ9QYQCFi7"));

            Assert.IsTrue(mockWalletManager.Wallet.Accounts.ToList().Count == 1);
        }

        [TestMethod]
        [ExpectedException(typeof(WalletNotOpenException))]
        public void TestImportWifWalletIsNotOpened()
        {
            var mockWalletManager = GetAWalletManagerWithoutAnWallet();

            // Act
            IWalletAccount walletAccount = mockWalletManager.ImportWif("KxLNhtdXXqaYUW1DKBc1XYQLxhouxXPLgQhR8kk7SYG3ajjR8M8a", _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestImportWifNull()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            mockWalletManager.ImportPrivateKey(new byte[] { }, _defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestImportWifEmpty()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            mockWalletManager.ImportWif(String.Empty, _defaultPassword);
        }

        #endregion

        #region Check Password

        [TestMethod]
        [ExpectedException(typeof(AccountsPasswordMismatchException))]
        public void TestVerifyPasswordFalse()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();
            mockWalletManager.CreateAndAddAccount(_defaultPassword);
            // Act
            SecureString fakeString = new SecureString();
            fakeString.AppendChar('1');
            mockWalletManager.CheckIfPasswordMatchesOpenWallet(fakeString);
        }

        [TestMethod]
        public void TestVerifyPassword()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();
            mockWalletManager.CheckIfPasswordMatchesOpenWallet(_defaultPassword);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestVerifyPasswordPasswordNull()
        {
            var mockWalletManager = GetAWalletManagerWithAnWallet();

            // Act
            mockWalletManager.CheckIfPasswordMatchesOpenWallet(null);
        }

        #endregion

        #region Private Methods

        private byte[] GetPrivateKeyFromWIF(string wif)
        {
            if (wif == null)
            {
                throw new ArgumentNullException();
            }

            byte[] data = Crypto.Default.Base58CheckDecode(wif);

            if (data.Length != 34 || data[0] != 0x80 || data[33] != 0x01)
            {
                throw new FormatException();
            }

            byte[] privateKey = new byte[32];
            Buffer.BlockCopy(data, 1, privateKey, 0, privateKey.Length);
            Array.Clear(data, 0, data.Length);
            return privateKey;
        }

        private Nep6WalletManager GetAWalletManagerWithoutAnWallet()
        {
            AutoMockContainer.Register<IFileWrapper>(new FileWrapper());
            AutoMockContainer.Register<IJsonConverter>(new JsonConverterWrapper());
            return AutoMockContainer.Create<Nep6WalletManager>();
        }

        private Nep6WalletManager GetAWalletManagerWithAnWallet()
        {
            Random random = new Random();
            String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            String fileName = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());

            var mock = GetAWalletManagerWithoutAnWallet();
            mock.CreateWallet(fileName);
            return mock;
        }

        #endregion
    }
}
