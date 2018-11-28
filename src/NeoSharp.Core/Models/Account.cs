using System;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Account : ICloneable<Account>
    {
        [BinaryProperty(1)]
        [JsonProperty("scripthash")]
        public UInt160 ScriptHash;

        [BinaryProperty(2)]
        [JsonProperty("isfrozen")]
        public bool IsFrozen;

        [BinaryProperty(2)]
        [JsonProperty("votes")]
        public ECPoint[] Votes;

        [BinaryProperty(3)]
        [JsonProperty("balances")]
        public Dictionary<UInt256, Fixed8> Balances;

        public Account()
        {
            Balances = new Dictionary<UInt256, Fixed8>();
            Votes = Array.Empty<ECPoint>();
        }

        public Account(UInt160 scriptHash) : this()
        {
            ScriptHash = scriptHash;
        }

        public Account Clone()
        {
            return new Account
            {
                ScriptHash = ScriptHash,
                IsFrozen = IsFrozen,
                Votes = Votes,
                Balances = Balances.ToDictionary(p => p.Key, p => p.Value)
            };
        }

        public void FromReplica(Account replica)
        {
            ScriptHash = replica.ScriptHash;
            IsFrozen = replica.IsFrozen;
            Votes = replica.Votes;
            Balances = replica.Balances;
        }
    }
}