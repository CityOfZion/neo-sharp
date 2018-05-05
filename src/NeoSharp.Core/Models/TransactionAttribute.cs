using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class TransactionAttribute
    {
        [BinaryProperty(1)]
        [JsonProperty("usage")]
        public TransactionAttributeUsage Usage;

        [BinaryProperty(2)]
        [JsonProperty("data")]
        public string Data;
    }
}