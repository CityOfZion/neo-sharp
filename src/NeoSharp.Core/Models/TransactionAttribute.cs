using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    [BinaryTypeSerializer(typeof(TransactionAttributeConverter))]
    public class TransactionAttribute
    {
        [BinaryProperty(0)]
        [JsonProperty("usage")]
        public TransactionAttributeUsage Usage;

        [BinaryProperty(1)]
        [JsonProperty("data")]
        public byte[] Data;
    }
}