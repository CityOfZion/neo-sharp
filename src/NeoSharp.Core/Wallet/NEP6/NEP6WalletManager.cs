using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.SmartContract;
using NeoSharp.Core.Wallet.Exceptions;
using NeoSharp.Core.Wallet.Helpers;
using NeoSharp.Core.Wallet.Wrappers;
using NeoSharp.Cryptography;
using NeoSharp.Types;


namespace NeoSharp.Core.Wallet.NEP6
{
    public class Nep6WalletManager : IWalletManager
    {
        private readonly WalletHelper _walletHelper;
        private readonly IFileWrapper _fileWrapper;
        private readonly IJsonConverter _jsonConverter;
        /// <summary>
        /// Nep2Key to (publicKey, privateKey)
        /// </summary>
        private readonly IDictionary<string, Tuple<ECPoint, byte[]>> _unlockedAccounts = new Dictionary<String, Tuple<ECPoint, byte[]>>();
        private byte[] _accountPasswordHashCache;
        private string _openWalletFilename;
        public IWallet Wallet { get; private set; }

        public Nep6WalletManager(IFileWrapper fileWrapper, IJsonConverter jsonConverter)
        {
            _walletHelper = new WalletHelper();
            _fileWrapper = fileWrapper;
            _jsonConverter = jsonConverter;
        }



        /// <inheritdoc />
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

            Wallet = wallet;
            _openWalletFilename = filename;

            SaveWallet();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IWalletAccount CreateAndAddAccount(SecureString password)
        {
            var walletAccount = CreateAccount(password);
            AddAccount(walletAccount);
            return walletAccount;
        }

        /// <inheritdoc />
        public IWalletAccount CreateAccount(SecureString password)
        {
            CheckWalletIsOpen();
            CheckIfPasswordMatchesOpenWallet(password);

            var privateKey = Crypto.Default.GenerateRandomBytes(32);
            var account = ImportPrivateKey(privateKey, password);
            Array.Clear(privateKey, 0, privateKey.Length);
            _accountPasswordHashCache = GetPasswordHash(password);
            return account;
        }

        /// <inheritdoc />
        public void DeleteAccount(UInt160 scriptHash)
        {
            if (scriptHash == null)
            {
                throw new ArgumentNullException();
            }
            CheckWalletIsOpen();

            Wallet.Accounts = Wallet.Accounts.Where(x => !x.Contract.ScriptHash.Equals(scriptHash)).ToArray();
            SaveWallet();
        }

        public IWalletAccount GetAccount(string alias)
        {
            if (alias == null)
            {
                throw new ArgumentNullException();
            }
            CheckWalletIsOpen();

            return Wallet.Accounts
                 .Where(x => x.Label.Equals(alias))
                 .FirstOrDefault();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IWalletAccount GetAccount(ECPoint pubkey)
        {
            if (pubkey == null)
            {
                throw new ArgumentNullException();
            }

            var scriptHash = _walletHelper.ScriptHashFromPublicKey(pubkey);

            return GetAccount(scriptHash);
        }

        /// <inheritdoc />
        public void UpdateAccountAlias(UInt160 scripthash, string alias)
        {
            var account = GetAccount(scripthash);
            account.Label = alias;
            SaveWallet();
        }

        /// <inheritdoc />
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

            //TODO #360: Load Contract from persistence?
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public byte[] GetPrivateKeyFromWIF(string wif)
        {
            return _walletHelper.GetPrivateKeyFromWIF(wif);
        }

        /// <inheritdoc />
        public string PrivateKeyToWif(byte[] privateKey)
        {
            return _walletHelper.PrivateKeyToWif(privateKey);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IWalletAccount ImportEncryptedWif(string nep2, SecureString passphrase)
        {
            CheckWalletIsOpen();
            var privateKey = _walletHelper.DecryptWif(nep2, passphrase);
            var account = CreateAccountWithPrivateKey(privateKey, passphrase);
            account.Key = nep2;
            AddAccount(account);
            return account;
        }


        /// <inheritdoc />
        public void ExportWallet(String filename)
        {
            CheckWalletIsOpen();
            var json = _jsonConverter.SerializeObject(Wallet);
            if (String.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentException("Serialization failed");
            }
            _fileWrapper.WriteToFile(json, filename);
        }


        /// <inheritdoc />
        public void Load(string fileName)
        {
            var json = _fileWrapper.Load(fileName);
            Wallet = _jsonConverter.DeserializeObject<NEP6Wallet>(json);
            _openWalletFilename = fileName;
        }

        /// <inheritdoc />
        public void Close()
        {
            Wallet = null;
            _openWalletFilename = null;
            _unlockedAccounts.Clear();
            _accountPasswordHashCache = null;
        }

        /// <inheritdoc />
        public void UnlockAccount(string nep2Key, SecureString password)
        {
            var privateKey = _walletHelper.DecryptWif(nep2Key, password);
            _accountPasswordHashCache = GetPasswordHash(password);
            var publicKeyInBytes = Crypto.Default.ComputePublicKey(privateKey, true);
            var publicKeyEcPoint = new ECPoint(publicKeyInBytes);
            UnlockAccount(nep2Key, publicKeyEcPoint, privateKey);
        }

        /// <inheritdoc />
        public ECPoint GetPublicKeyFromNep2(string nep2Key, SecureString password)
        {
            var privateKey = _walletHelper.DecryptWif(nep2Key, password);
            var publicKeyInBytes = Crypto.Default.ComputePublicKey(privateKey, true);
            return new ECPoint(publicKeyInBytes);
        }

        /// <inheritdoc />
        public byte[] DecryptNep2(string nep2key, SecureString keyPassword)
        {
            return _walletHelper.DecryptWif(nep2key, keyPassword);
        }

        /// <inheritdoc />
        public string EncryptNep2(byte[] privateKey, SecureString keyPassword)
        {
            return _walletHelper.EncryptWif(privateKey, keyPassword);
        }

        /// <inheritdoc />
        public void CheckWalletIsOpen()
        {
            if (Wallet == null)
            {
                throw new WalletNotOpenException();
            }
        }

        /// <inheritdoc />
        public void CheckIfPasswordMatchesOpenWallet(SecureString password)
        {
            CheckWalletIsOpen();
            byte[] passwordHash = GetPasswordHash(password);

            if (_accountPasswordHashCache != null)
            {
                if (!passwordHash.SequenceEqual(_accountPasswordHashCache))
                {
                    throw new AccountsPasswordMismatchException();
                }
            }

            if (Wallet.Accounts != null && Wallet.Accounts.Length > 0)
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

        /// <summary>
        /// save the open wallet into the same file
        /// </summary>
        public void SaveWallet()
        {
            CheckWalletIsOpen();
            ExportWallet(_openWalletFilename);
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

                List<IWalletAccount> accounts = new List<IWalletAccount>(Wallet.Accounts);
                accounts.Remove(account);
                Wallet.Accounts = accounts.ToArray();

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

            var accountList = Wallet.Accounts.ToList();

            if(!accountList.Contains(account))
            {
                accountList.Add(account);
                Wallet.Accounts = accountList.ToArray();
                SaveWallet();
            }
        }

        /// <summary>
        /// Creates the NEP6Account with private key.
        /// </summary>
        /// <returns>The NEP6Account</returns>
        /// <param name="privateKey">Private key.</param>
        private NEP6Account CreateAccountWithPrivateKey(byte[] privateKey, SecureString passphrase, string label = null)
        {
            var publicKeyInBytes = Crypto.Default.ComputePublicKey(privateKey, true);
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
        /// Hashs the password (twice) and returns the result.
        /// </summary>
        /// <returns>The password hash.</returns>
        /// <param name="password">Password.</param>
        private byte[] GetPasswordHash(SecureString password)
        {
            return Crypto.Default.Sha256(Crypto.Default.Sha256(password.ToByteArray()));
        }

        /// <summary>
        /// Unlocks the account and add it to the unlocked accounts.
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
    }
}