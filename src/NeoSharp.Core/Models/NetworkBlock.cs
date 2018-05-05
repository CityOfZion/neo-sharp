using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class NetworkBlock : Block
    {
        [BinaryProperty(11)]
        [JsonProperty("tx")]
        public Transaction[] Transactions;
    }
}