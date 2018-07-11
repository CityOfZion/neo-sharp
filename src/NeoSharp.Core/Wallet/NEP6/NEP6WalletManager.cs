using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.SmartContract;
using NeoSharp.Core.Types;
using NeoSharp.Core.Wallet.Exceptions;
using NeoSharp.Core.Wallet.Helpers;
using NeoSharp.Core.Wallet.Wrappers;


namespace NeoSharp.Core.Wallet.NEP6
{
    public class Nep6WalletManager : IWalletManager
    {

        private readonly WalletHelper _walletHelper;
        private readonly IFileWrapper _fileWrapper;
        private readonly IJsonConverter _jsonConverter;

        private string _openWalletFilename;
        public IWallet Wallet { get; private set; }

        /// <summary>
        /// Nep2Key to (publicKey, privateKey)
        /// </summary>
        private readonly IDictionary<string, Tuple<ECPoint, byte[]>> _unlockedAccounts = new Dictionary<String, Tuple<ECPoint, byte[]>>();

        private byte[] _accountPasswordHashCache;

        public Nep6WalletManager(IFileWrapper fileWrapper, IJsonConverter jsonConverter)
        {
            _walletHelper = new WalletHelper();
            _fileWrapper = fileWrapper;
            _jsonConverter = jsonConverter;
        }

        /// <summary>
        /// Creates the wallet.
        /// </summary>
        /// <returns>The wallet.</returns>
        /// <param name="filename">File name.</param>
        public void CreateWallet(string filename)
        {
            if (_fileWrapper.Exists(filename)) 
            {
                throw new ArgumentException("File already exists");
            }

            var walletName = Path.GetFileNameWithoutExtension(filename);
            var wallet = new NEP6Wallet()
            {
                Name = walletName,
                Version = "1.0"
            };

            var json = _jsonConverter.SerializeObject(wallet);
            _fileWrapper.WriteToFile(json, filename);

            Wallet = wallet;
            _openWalletFilename = filename;
        }

        /// <summary>
        /// Check if Accounts contains a script hash
        /// </summary>
        /// <returns></returns>
        /// <param name="scriptHash"></param>
        public bool Contains(UInt160 scriptHash)
        {
            if (scriptHash == null)
            {
                throw new ArgumentNullException();
            }

            CheckWalletIsOpen();

            return Wallet.Accounts
               .Where(x => x.Contract.ScriptHash.Equals(scriptHash))
               .FirstOrDefault() != null;
        }

        /// <summary>
        /// Creates the account with random private key.
        /// </summary>
        /// <returns>The account.</returns>
        public IWalletAccount CreateAccount(SecureString password)
        {
            CheckWalletIsOpen();
            byte[] passwordHash;
            ValidateAccountsPasswordMismatch(password, out passwordHash);

            var privateKey = ICrypto.Default.GenerateRandomBytes(32);
            var account = ImportPrivateKey(privateKey, password);
            Array.Clear(privateKey, 0, privateKey.Length);
            _accountPasswordHashCache = passwordHash;
            return account;
        }

        /// <summary>
        /// Remove the account.
        /// </summary>
        /// <param name="scriptHash">Scripthash's account.</param>
        public void DeleteAccount(UInt160 scriptHash)
        {
            if (scriptHash == null)
            {
                throw new ArgumentNullException();
            }
            CheckWalletIsOpen();
            
            Wallet.Accounts = Wallet.Accounts.Where(x => !x.Contract.ScriptHash.Equals(scriptHash)).ToHashSet();
            SaveWallet();
        }

        /// <summary>
        /// Gets the account.
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="scriptHash">Scripthash's account.</param>
        public IWalletAccount GetAccount(UInt160 scriptHash)
        {
            if (scriptHash == null)
            {
                throw new ArgumentNullException();
            }
            CheckWalletIsOpen();

            return Wallet.Accounts
                .Where(x => x.Contract.ScriptHash.Equals(scriptHash))
                .FirstOrDefault();
        }

        /// <summary>
        /// Gets the account using public key.
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="pubkey">Pubkey.</param>
        public IWalletAccount GetAccount(ECPoint pubkey)
        {
            if (pubkey == null)
            {
                throw new ArgumentNullException();
            }

            var scriptHash = _walletHelper.ScriptHashFromPublicKey(pubkey);

            return GetAccount(scriptHash);
        }

        /// <summary>
        /// Instantiates the account and adds to the Accounts
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="scriptHash">Script hash.</param>
        public IWalletAccount ImportScriptHash(UInt160 scriptHash)
        {
            if (scriptHash == null)
            {
                throw new ArgumentNullException();
            }

            if (scriptHash.Equals(UInt160.Zero))
            {
                throw new ArgumentException();
            }
            CheckWalletIsOpen();

            //TODO: Load Contract from persistence?
            var emptyContractCode = new Code
            {
                ScriptHash = scriptHash
            };
            var emptyContract = new Contract
            {
                Code = emptyContractCode
            };

            var account = new NEP6Account(emptyContract);

            AddAccount(account);
            return account;
        }

        /// <summary>
        /// Import the account using private key
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="privateKey">Private key.</param>
        public IWalletAccount ImportPrivateKey(byte[] privateKey, SecureString passphrase)
        {
            if (privateKey == null)
            {
                throw new ArgumentNullException();
            }

            if (privateKey.Length == 0)
            {
                throw new ArgumentException();
            }
            CheckWalletIsOpen();

            var account = CreateAccountWithPrivateKey(privateKey, passphrase);
            AddAccount(account);
            return account;
        }

        /// <summary>
        /// Import the Account using wif.
        /// </summary>
        /// <returns>The import.</returns>
        /// <param name="wif">Wif.</param>
        public IWalletAccount ImportWif(string wif, SecureString password)
        {
            if (string.IsNullOrWhiteSpace(wif))
            {
                throw new ArgumentNullException();
            }
            CheckWalletIsOpen();

            var account = CreateAccountWithPrivateKey(GetPrivateKeyFromWIF(wif), password);
            AddAccount(account);
            return account;
        }

        /// <summary>
        /// Import the Account using nep2 and passphrase.
        /// </summary>
        /// <returns>The import.</returns>
        /// <param name="nep2">Nep2.</param>
        /// <param name="passphrase">Passphrase.</param>
        public IWalletAccount ImportEncryptedWif(string nep2, SecureString passphrase)
        {
            CheckWalletIsOpen();
            var privateKey = _walletHelper.DecryptWif(nep2, passphrase);
            var account = CreateAccountWithPrivateKey(privateKey, passphrase);
            account.Key = nep2;
            AddAccount(account);
            return account;
        }

        /// <summary>
        /// Verifies the password.
        /// </summary>
        /// <returns><c>true</c>, if password was verifyed, <c>false</c> otherwise.</returns>
        /// <param name="walletAccout">Wallet accout.</param>
        /// <param name="password">Password.</param>
        public bool VerifyPassword(IWalletAccount walletAccout, SecureString password)
        {
            if (walletAccout == null)
            {
                throw new ArgumentException();
            }

            try
            {
                _walletHelper.DecryptWif(walletAccout.Key, password);
            }

            catch (FormatException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the private key from wif.
        /// </summary>
        /// <returns>The private key from wif.</returns>
        /// <param name="wif">Wif.</param>
        private byte[] GetPrivateKeyFromWIF(string wif)
        {
            var internalWif = wif ?? throw new ArgumentNullException(nameof(wif));

            var privateKeyByteArray = ICrypto.Default.Base58CheckDecode(internalWif);

            if(privateKeyByteArray.IsValidPrivateKey()){
                var privateKey = new byte[32];
                Buffer.BlockCopy(privateKeyByteArray, 1, privateKey, 0, privateKey.Length);
                Array.Clear(privateKeyByteArray, 0, privateKeyByteArray.Length);
                return privateKey;
            }else{
                throw new FormatException();
            }
        }

        /// <summary>
        /// save the open wallet into the same file
        /// </summary>
        public void SaveWallet()
        {
            SaveWallet(_openWalletFilename);
        }

        /// <summary>
        /// save the open wallet into a specific filename
        /// </summary>
        /// <param name="filename">the filename</param>
        public void SaveWallet(String filename)
        {
            CheckWalletIsOpen();
            var json = _jsonConverter.SerializeObject(Wallet);
            _fileWrapper.WriteToFile(json, filename);
        }

        /// <summary>
        /// Adds the or replace an account in the system.
        /// If the account with that scriptHash already exists, 
        /// it's going to be replaced
        /// </summary>
        /// <param name="account">Account.</param>
        private void AddOrReplaceAccount(IWalletAccount account)
        {
            CheckWalletIsOpen();
            var currentAccount = TryGetAccount(account.Contract.ScriptHash);
            if (currentAccount == null)
            {
                AddAccount(account);
            }
            else
            {
                //Account exists. Clone it.
                var clonedAccount = new NEP6Account(account.Contract)
                {
                    Label = account.Label,
                    IsDefault = account.IsDefault,
                    Extra = account.Extra,
                    Key = account.Key,
                    Lock = account.Lock
                };

                Wallet.Accounts.Remove(account);

                AddAccount(clonedAccount);
            }

            SaveWallet();
        }

        /// <summary>
        /// Retrieves the account from the Account list using the script hash/
        /// Replaces the account information with those provided in the newAccountInformation parameter
        /// Returns the account but does not save it
        /// This should be used when the user wants to update a current account,
        /// like replacing the paraters from the contract or replacing the label.
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="newAccountInformation">Account script hash.</param>
        private IWalletAccount CloneAndUpdate(IWalletAccount newAccountInformation)
        {
            newAccountInformation.ValidateAccount();

            var currentAccountWithScriptHash = TryGetAccount(newAccountInformation.Contract.ScriptHash);
            if (currentAccountWithScriptHash == null)
            {
                throw new ArgumentException("Account not found.");
            }

            var clonedAccount = new NEP6Account(newAccountInformation.Contract)
            {
                Label = newAccountInformation.Label,
                IsDefault = newAccountInformation.IsDefault,
                Lock = newAccountInformation.Lock,
            };

            return clonedAccount;
        }

        /// <summary>
        /// Adds the account to the Account list.
        /// </summary>
        /// <param name="account">Account.</param>
        private void AddAccount(IWalletAccount account)
        {
            var internalAccount = account ?? throw new ArgumentException(nameof(account));

            account.ValidateAccount();

            //Accounts is a set, it cannot contain duplicates.
            Wallet.Accounts.Add(account);
            SaveWallet();
        }


        /// <summary>
        /// Creates the NEP6Account with private key.
        /// </summary>
        /// <returns>The NEP6Account</returns>
        /// <param name="privateKey">Private key.</param>
        private NEP6Account CreateAccountWithPrivateKey(byte[] privateKey, SecureString passphrase, string label = null)
        {
            var publicKeyInBytes = ICrypto.Default.ComputePublicKey(privateKey, true);
            var publicKeyInEcPoint = new ECPoint(publicKeyInBytes);
            var contract = ContractFactory.CreateSinglePublicKeyRedeemContract(publicKeyInEcPoint);

            var account = new NEP6Account(contract)
            {
                Label = label
            };

            account.Key = _walletHelper.EncryptWif(privateKey, passphrase);
            UnlockAccount(account.Key, publicKeyInEcPoint, privateKey);
            return account;
        }

        /// <summary>
        /// Tries the get account from the Account list
        /// </summary>
        /// <returns>The account found with that ScriptHash</returns>
        /// <param name="accountScriptHash">Account ScriptHash.</param>
        private IWalletAccount TryGetAccount(UInt160 accountScriptHash)
        {
            return Wallet?.Accounts
                          .Where(x => x.Contract.ScriptHash.Equals(accountScriptHash))
                          .FirstOrDefault();
        }

        /// <summary>
        /// Load a wallet using a file name.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public void Load(string fileName)
        {
            var json = _fileWrapper.Load(fileName);
            Wallet = _jsonConverter.DeserializeObject<NEP6Wallet>(json);
            _openWalletFilename = fileName;
        }

        /// <summary>
        /// Close wallet.
        /// </summary>
        public void Close()
        {
            Wallet = null;
            _openWalletFilename = null;
            _unlockedAccounts.Clear();
            _accountPasswordHashCache = null;
        }

        /// <summary>
        /// Unlocks an account of the specified nep2key.
        /// </summary>
        /// <param name="nep2Key">Nep2 key.</param>
        /// <param name="password">Password.</param>
        public void UnlockAccount(string nep2Key, SecureString password)
        {
            var privateKey = _walletHelper.DecryptWif(nep2Key, password);
            _accountPasswordHashCache = GetPasswordHash(password);
            var publicKeyInBytes = ICrypto.Default.ComputePublicKey(privateKey, true);
            var publicKeyEcPoint = new ECPoint(publicKeyInBytes);
            UnlockAccount(nep2Key, publicKeyEcPoint, privateKey);
        }


        /// <summary>
        /// Stores the public / private key into an dictionary, using the encrypted wif (nep2) as key
        /// </summary>
        /// <param name="nep2Key">Nep2 key.</param>
        /// <param name="publicKey">Public key.</param>
        /// <param name="privateKey">Private key.</param>
        private void UnlockAccount(string nep2Key, ECPoint publicKey, byte[] privateKey)
        {
            var internalNep2Key = nep2Key ?? throw new ArgumentException(nameof(nep2Key));

            var entry = new Tuple<ECPoint, byte[]>(publicKey, privateKey);

            if (_unlockedAccounts.ContainsKey(nep2Key))
            {
                _unlockedAccounts[nep2Key] = entry;
            }
            else
            {
                _unlockedAccounts.Add(nep2Key, entry);
            }
        }

        /// <summary>
        /// Throw exception if wallet is not opened (aka null)
        /// </summary>
        public void CheckWalletIsOpen()
        {
            if (Wallet == null)
            {
                throw new WalletNotOpenException();
            }
        }
        
        private byte[] GetPasswordHash(SecureString password)
        {
            return ICrypto.Default.Sha256(ICrypto.Default.Sha256(password.ToByteArray()));
        }

        private void ValidateAccountsPasswordMismatch(SecureString password, out byte[] passwordHash)
        {
            CheckWalletIsOpen();

            passwordHash = GetPasswordHash(password);
            
            if (_accountPasswordHashCache != null) {
                if (!passwordHash.SequenceEqual(_accountPasswordHashCache))
                {
                    throw new AccountsPasswordMismatchException();
                }
            }
            else if (Wallet.Accounts != null && Wallet.Accounts.Count > 0)
            {
                try
                {
                    _walletHelper.DecryptWif(Wallet.Accounts.First().Key, password);
                }
                catch (FormatException)
                {
                    throw new AccountsPasswordMismatchException();
                }
            }
        }

    }
}