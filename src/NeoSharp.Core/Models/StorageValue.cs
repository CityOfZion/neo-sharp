using System;
using NeoSharp.BinarySerialization;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class StorageValue
    {
        [BinaryProperty(1)]
        [JsonProperty("value")]
        public byte[] Value;
    }
}