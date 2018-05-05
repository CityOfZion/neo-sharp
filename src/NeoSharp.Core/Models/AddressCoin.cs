using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class AddressCoin
    {
        [BinaryProperty(1)]
        [JsonProperty("id")]
        public string Id;

        [BinaryProperty(2)]
        [JsonProperty("assetid")]
        public string AssetId;

        [BinaryProperty(3)]
        [JsonProperty("coinstate")]
        public CoinState CoinState;
    }
}