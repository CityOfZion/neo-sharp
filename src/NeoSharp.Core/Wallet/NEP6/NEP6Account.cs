using System;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using NeoSharp.Core.Wallet.Helpers;
using Newtonsoft.Json;

namespace NeoSharp.Core.Wallet.NEP6
{
    public class NEP6Account : IWalletAccount, IEquatable<NEP6Account>
    {
        /// <inheritdoc />
        public string Address { get; set; }

        /// <inheritdoc />
        public UInt160 ScriptHash => Contract.ScriptHash;

        /// <inheritdoc />>
        public string Label { get; set; }

        /// <inheritdoc />
        public bool IsDefault { get; set; }

        /// <inheritdoc />
        public bool Lock { get; set; }

        /// <inheritdoc />
        public string Key { get; set; }

        /// <inheritdoc />
        public Contract Contract { get; set; }

        [JsonProperty("extra")]
        public Object Extra { get; set; }

        public NEP6Account(Contract accountContract){
            Contract = accountContract;
            Address = accountContract.ScriptHash.ToAddress();
        }


        public override bool Equals(object obj)
        {
            if (obj is NEP6Account acc)
            {
                return ScriptHash.Equals(acc.ScriptHash);
            }

            return false;
        }
         
        public bool Equals(NEP6Account obj)
        {
            return ScriptHash.Equals(obj);
        }

        public override int GetHashCode()
        {
            return ScriptHash.GetHashCode();
        }
    }
}
