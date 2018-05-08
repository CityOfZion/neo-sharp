using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Coin : TransactionOutput
    {
        [BinaryProperty(1)]
        [JsonProperty("id")]
        public string Id
        {
            get { return $"{TxId}_{Index}"; }
        }

        [BinaryProperty(2)]
        [JsonProperty("txid")]
        public string TxId;

        [BinaryProperty(3)]
        [JsonProperty("assetname")]
        public string AssetName;

        [BinaryProperty(4)]
        [JsonProperty("precision")]
        public byte AssetPrecision;

        [BinaryProperty(5)]
        [JsonProperty("tracedhash")]
        public string TracedHash;

        [BinaryProperty(6)]
        [JsonProperty("coinstate")]
        public CoinState CoinState;
    }
}