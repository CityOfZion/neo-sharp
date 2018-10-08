using NeoSharp.Core.Cryptography;
using Newtonsoft.Json.Linq;

namespace NeoSharp.Core.Wallet
{
    /// <summary>
    /// Wallet. <see href="https://github.com/neo-project/proposals/blob/master/nep-6.mediawiki#Wallet"/>
    /// </summary>
    public interface IWallet
    {
        /// <summary>
        /// name is a label that the user has made to the wallet file.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Version is currently fixed at 1.0 and will be used for functional 
        /// upgrades in the future
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// scrypt is a ScryptParameters object which describe the parameters 
        /// of SCrypt algorithm used for encrypting and decrypting the private
        /// keys in the wallet.
        /// </summary>
        ScryptParameters Scrypt { get; set; }

        /// <summary>
        /// accounts is an list of Account objects which describe the details 
        /// of each account in the wallet.
        /// </summary>
        IWalletAccount[] Accounts { get; set; }

        /// <summary>
        /// extra is an object that is defined by the implementor of the client
        /// for storing extra data. This field can be null.
        /// </summary>
        JObject Extra { get; set; }
    }
}
