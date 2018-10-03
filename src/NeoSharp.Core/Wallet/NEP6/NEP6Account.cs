using System;
using NeoSharp.Core.Models;
using NeoSharp.Core.Wallet.Helpers;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Wallet.NEP6
{
    public class NEP6Account : IWalletAccount, IEquatable<NEP6Account>
    {
        /// <inheritdoc />
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <inheritdoc />
        [JsonIgnore]
        public UInt160 ScriptHash => Contract.ScriptHash;

        /// <inheritdoc />>
        [JsonProperty("label")]
        public string Label { get; set; }

        /// <inheritdoc />
        [JsonProperty("isDefault")]
        public bool IsDefault { get; set; }

        /// <inheritdoc />
        [JsonProperty("lock")]
        public bool Lock { get; set; }

        /// <inheritdoc />
        [JsonProperty("key")]
        public string Key { get; set; }

        /// <inheritdoc />
        [JsonProperty("contract")]
        public Contract Contract { get; set; }

        [JsonProperty("extra")]
        public Object Extra { get; set; }

        public NEP6Account()
        {
        }

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
