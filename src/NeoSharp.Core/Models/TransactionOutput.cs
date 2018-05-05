using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class TransactionOutput
    {
        [BinaryProperty(1)]
        [JsonProperty("n")]
        public string Index;

        [BinaryProperty(2)]
        [JsonProperty("asset")]
        public string AssetId;

        [BinaryProperty(3)]
        [JsonProperty("value")]
        public double Value;

        [BinaryProperty(4)]
        [JsonProperty("address")]
        public string Address;
    }
}