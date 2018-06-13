using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    [BinaryTypeSerializer(typeof(TransactionAttributeBinarySerializer))]
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