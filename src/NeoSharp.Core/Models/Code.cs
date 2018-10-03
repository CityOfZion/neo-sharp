using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.SmartContract;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Code
    {
        [BinaryProperty(1)]
        [JsonProperty("hash")]
        public UInt160 ScriptHash;

        [BinaryProperty(2)]
        [JsonProperty("script")]
        public byte[] Script;

        [BinaryProperty(3)]
        [JsonProperty("returntype")]
        public ContractParameterType ReturnType;

        [BinaryProperty(4)]
        [JsonProperty("parameters")]
        public ContractParameterType[] Parameters = new ContractParameterType[0];
    }
}