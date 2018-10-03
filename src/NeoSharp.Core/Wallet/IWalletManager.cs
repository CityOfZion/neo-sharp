using System.Security;
using NeoSharp.Core.Cryptography;
using NeoSharp.Types;

namespace NeoSharp.Core.Wallet
{
    public interface IWalletManager
    {
        /// <summary>
        /// the open wallet 
        /// </summary>
        IWallet Wallet { get; }

        /// <summary>
        /// Creates the wallet and saves the instance.
        /// </summary>
        /// <returns>The wallet.</returns>
        /// <param name="filename">File name.</param>
        void CreateWallet(string filename);

        /// <summary>
        /// Check if Accounts contains a script hash
        /// </summary>
        /// <returns></returns>
        /// <param name="scriptHash"></param>
        bool Contains(UInt160 scriptHash);

        /// <summary>
        /// Creates the account with random private key. 
        /// It does not add it to the user Wallet.
        /// </summary>
        /// <returns>The account.</returns>
        IWalletAccount CreateAccount(SecureString password);

        /// <summary>
        /// Creates the account with random private key 
        /// and adds it to the current Wallet.
        /// </summary>
        /// <returns>The account.</returns>
        IWalletAccount CreateAndAddAccount(SecureString password);

        /// <summary>
        /// Remove the account.
        /// </summary>
        /// <param name="scriptHash">Scripthash's account.</param>
        void DeleteAccount(UInt160 scriptHash);

        /// <summary>
        /// Gets the account using the account <paramref name="alias"/>(label property).
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="scriptHash">Scripthash's account.</param>
        IWalletAccount GetAccount(string alias);

        /// <summary>
        /// Gets the account using the account scripthash.
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="scriptHash">Scripthash's account.</param>
        IWalletAccount GetAccount(UInt160 scriptHash);

        /// <summary>
        /// Gets the account using public key.
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="pubkey">Pubkey.</param>
        IWalletAccount GetAccount(ECPoint pubkey);

        /// <summary>
        /// Import the account using script hash
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="scriptHash">Script hash.</param>
        IWalletAccount ImportScriptHash(UInt160 scriptHash);

        /// <summary>
        /// Import the account using private key
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="privateKey">Private key.</param>
        IWalletAccount ImportPrivateKey(byte[] privateKey, SecureString password);

        /// <summary>
        /// Import the Account using wif.
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="wif">Wif.</param>
        IWalletAccount ImportWif(string wif, SecureString password);

        /// <summary>
        /// Unlocks an account of the specified nep2key.
        /// </summary>
        /// <param name="nep2Key">Nep2 key.</param>
        /// <param name="password">Password.</param>
        void UnlockAccount(string nep2Key, SecureString password);

        /// <summary>
        /// Get a PublicKey from <paramref name="nep2Key"/>.
        /// </summary>
        /// <returns>The public key from nep2.</returns>
        /// <param name="nep2Key">Nep2 key.</param>
        /// <param name="password">Password.</param>
        ECPoint GetPublicKeyFromNep2(string nep2Key, SecureString password);

        /// <summary>
        /// Import the Account using nep2 and passphrase.
        /// </summary>
        /// <returns>The account.</returns>
        /// <param name="nep2">Nep2.</param>
        /// <param name="password">Password.</param>
        IWalletAccount ImportEncryptedWif(string nep2, SecureString password);

        /// <summary>
        /// Saves the current wallet into the HD
        /// </summary>
        void SaveWallet();

        /// <summary>
        /// Checks the wallet is open.
        /// </summary>
        void CheckWalletIsOpen();

        /// <summary>
        /// Check if the password provided is the same used in the first wallet account.
        /// </summary>
        void CheckIfPasswordMatchesOpenWallet(SecureString password);

        /// <summary>
        /// save the open wallet into a specific filename
        /// </summary>
        /// <param name="filename">the filename</param>
        void ExportWallet(string filename);

        /// <summary>
        /// Returns the private key from a NEP2 key and password
        /// </summary>
        /// <returns>The nep2 key.</returns>
        /// <param name="nep2key">Nep2key.</param>
        /// <param name="keyPassword">Key password.</param>
        byte[] DecryptNep2(string nep2key, SecureString keyPassword);

        /// <summary>
        /// Returns the encrypted private key (nep-2)
        /// </summary>
        /// <returns>The nep2.</returns>
        /// <param name="privateKey">Private key.</param>
        /// <param name="keyPassword">Key password.</param>
        string EncryptNep2(byte[] privateKey, SecureString keyPassword);

        /// <summary>
        /// Load a wallet at specified fileName.
        /// </summary>
        /// <param name="fileName">File name.</param>
        void Load(string fileName);

        /// <summary>
        /// Close wallet.
        /// </summary>
        void Close();

        /// <summary>
        /// Converts a byte array into a wif string
        /// </summary>
        /// <returns>Private key in wif format.</returns>
        /// <param name="privateKey">Private key.</param>
        string PrivateKeyToWif(byte[] privateKey);

        /// <summary>
        /// Gets the private key from wif.
        /// </summary>
        /// <returns>The private key from wif.</returns>
        /// <param name="wif">Wif.</param>
        byte[] GetPrivateKeyFromWIF(string wif);

        /// <summary>
        /// Adds/replaces an account alias (label) 
        /// </summary>
        /// <param name="scripthash">Account scripthash.</param>
        /// <param name="alias">Account label</param>
        void UpdateAccountAlias(UInt160 scripthash, string alias);
    }
}
