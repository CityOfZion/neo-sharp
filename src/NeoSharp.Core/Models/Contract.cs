using NeoSharp.BinarySerialization;
using NeoSharp.Core.SmartContract;
using Newtonsoft.Json;
using System;
using System.Linq;
using NeoSharp.Core.Types;
using NeoSharp.Types;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Contract : ICloneable<Contract>
    {
        public UInt160 ScriptHash => Code?.ScriptHash;

        public byte[] Script => Code?.Script;

        public ContractParameterType[] Parameters => Code?.Parameters;

        public ContractParameterType ReturnType => Code?.ReturnType ?? ContractParameterType.Void;

        public bool HasStorage => Code?.Metadata.HasFlag(ContractMetadata.HasStorage) ?? false;

        public bool HasDynamicInvoke => Code?.Metadata.HasFlag(ContractMetadata.HasDynamicInvoke) ?? false;

        public bool Payable => Code?.Metadata.HasFlag(ContractMetadata.Payable) ?? false;

        [BinaryProperty(1)]
        [JsonProperty("code")]
        public Code Code { get; set; }

        [BinaryProperty(2)]
        [JsonProperty("name")]
        public string Name;

        [BinaryProperty(3)]
        [JsonProperty("version")]
        public string Version;

        [BinaryProperty(4)]
        [JsonProperty("author")]
        public string Author;

        [BinaryProperty(5)]
        [JsonProperty("email")]
        public string Email;

        [BinaryProperty(6)]
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

        public Contract Clone()
        {
            return new Contract
            {
                Code = new Code
                {
                    Script = Code?.Script,
                    ScriptHash = Code?.ScriptHash,
                    Parameters = Code?.Parameters.ToArray(),
                    ReturnType = Code?.ReturnType ?? ContractParameterType.Void,
                    Metadata = Code?.Metadata ?? ContractMetadata.NoProperty
                },
                Name = Name,
                Version = Version,
                Author = Author,
                Email = Email,
                Description = Description
            };
        }

        public void FromReplica(Contract replica)
        {
            Code = new Code
            {
                Script = replica.Code?.Script,
                ScriptHash = replica.Code?.ScriptHash,
                Parameters = replica.Code?.Parameters.ToArray(),
                ReturnType = replica.Code?.ReturnType ?? ContractParameterType.Void,
                Metadata = Code?.Metadata ?? ContractMetadata.NoProperty
            };
            Name = replica.Name;
            Version = replica.Version;
            Author = replica.Author;
            Email = replica.Email;
            Description = replica.Description;
        }
    }
}