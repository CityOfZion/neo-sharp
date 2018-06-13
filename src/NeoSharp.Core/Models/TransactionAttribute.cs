using System;
using NeoSharp.BinarySerialization;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class TransactionAttribute
    {
        [BinaryProperty(0)]
        [JsonProperty("usage")]
        public TransactionAttributeUsage Usage;

        [BinaryProperty(1)]
        [JsonProperty("data")]
        public byte[] Data;

        // TODO: https://github.com/neo-project/neo/blob/master/neo/Core/TransactionAttribute.cs
    }
}