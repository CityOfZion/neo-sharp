using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class CoinReference
    {
        [BinaryProperty(1)]
        [JsonProperty("txid")]
        public UInt256 PrevHash;

        [BinaryProperty(2)]
        [JsonProperty("vout")]
        public ushort PrevIndex;

        [BinaryProperty(3)]
        [JsonProperty("id")]
        public string Id
        {
            get { return $"{PrevHash}_{PrevIndex}"; }
        }
    }
}