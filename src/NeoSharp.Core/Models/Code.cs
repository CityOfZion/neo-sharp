using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Code
    {
        [BinaryProperty(1)]
        [JsonProperty("hash")]
        public string Hash;

        [BinaryProperty(2)]
        [JsonProperty("script")]
        public string Script;

        [BinaryProperty(3)]
        [JsonProperty("returntype")]
        public string ReturnType;

        [BinaryProperty(4)]
        [JsonProperty("parameters")]
        public string[] Parameters = new string[0];
    }
}