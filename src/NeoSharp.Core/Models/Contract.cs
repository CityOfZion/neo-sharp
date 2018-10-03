using NeoSharp.BinarySerialization;
using NeoSharp.Core.SmartContract;
using Newtonsoft.Json;
using System;
using NeoSharp.Types;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Contract
    {
        public UInt160 ScriptHash => Code?.ScriptHash;
        public byte[] Script => Code?.Script;
        public ContractParameterType[] Parameters => Code?.Parameters;
        public ContractParameterType ReturnType => (ContractParameterType)Code?.ReturnType;

        [BinaryProperty(1)]
        [JsonProperty("code")]
        public Code Code { get; set; }

        [BinaryProperty(2)]
        [JsonProperty("needstorage")]
        public bool NeedStorage;

        [BinaryProperty(3)]
        [JsonProperty("name")]
        public string Name;

        [BinaryProperty(4)]
        [JsonProperty("version")]
        public string Version;

        [BinaryProperty(5)]
        [JsonProperty("author")]
        public string Author;

        [BinaryProperty(6)]
        [JsonProperty("email")]
        public string Email;

        [BinaryProperty(7)]
        [JsonProperty("description")]
        public string Description;

        /// <summary>
        /// Get Contract from hash
        /// </summary>
        /// <param name="contractHash">Contract hash</param>
        /// <returns>Return contract</returns>
        public static Contract GetContract(UInt160 contractHash)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invoke contract
        /// </summary>
        /// <param name="args">Argument</param>
        public Transaction CreateInvokeTransaction(params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}