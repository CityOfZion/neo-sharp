using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class TransactionOutput
    {
        [BinaryProperty(0)]
        [JsonProperty("asset")]
        public UInt256 AssetId;

        [BinaryProperty(1)]
        [JsonProperty("value")]
        public Fixed8 Value;

        [BinaryProperty(2)]
        [JsonProperty("address")]
        public UInt160 ScriptHash;
    }
}