using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class StorageKey
    {
        [BinaryProperty(1)]
        [JsonProperty("scripthash")]
        public UInt160 ScriptHash;

        [BinaryProperty(2)]
        [JsonProperty("key")]
        public byte[] Key;
    }
}