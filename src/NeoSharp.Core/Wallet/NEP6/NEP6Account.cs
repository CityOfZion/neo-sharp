using System;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Wallet.NEP6
{
    public class NEP6Account : IWalletAccount, IEquatable<NEP6Account>
    {

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
