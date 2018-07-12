using System;
using System.Collections.Generic;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Account
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

        // TODO: Add binary serialization for Dictionary
//        [BinaryProperty(3)]
        [JsonProperty("balances")]
        public Dictionary<UInt256, Fixed8> Balances;
    }
}