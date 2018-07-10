using System;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Wallet
{
    public interface IWalletAccount 
    {

        /// <summary>
        /// Address is 'A' + Base58(scripthash)
        /// </summary>
        string Address { get; set; }

        /// <summary>
        /// Account name
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// isDefault indicates whether the account is the default change
        /// account.
        /// </summary>
        bool IsDefault { get; set; }

        /// <summary>
        /// lock indicates whether the account is locked by user. The client
        /// shouldn't spend the funds in a locked account.
        /// </summary>
        bool Lock { get; set; }

        /// <summary>
        /// key is the private key of the account in the NEP-2 format. This
        /// field can be null (for watch-only address or non-standard address).
        /// </summary>
        string Key { get; }

        /// <summary>
        /// contract is a Contract object which describes the details of the
        /// contract. This field can be null (for watch-only address).
        /// </summary>
        Contract Contract { get; set; }

        /// <summary>
        /// extra is an object that is defined by the implementor of the client
        /// for storing extra data. This field can be null
        /// </summary>
        object Extra { get; set; }
    }
}
