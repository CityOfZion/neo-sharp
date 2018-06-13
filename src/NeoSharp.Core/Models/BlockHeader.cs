using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class BlockHeader : NeoEntityBase
    {
        [BinaryProperty(2)]
        [JsonProperty("size")]
        public int Size;

        [BinaryProperty(3)]
        [JsonProperty("version")]
        public byte Version;

        [BinaryProperty(4)]
        [JsonProperty("previousblockhash")]
        public UInt256 PreviousBlockHash;

        [BinaryProperty(5)]
        [JsonProperty("merkleroot")]
        public UInt256 MerkleRoot;

        [BinaryProperty(6)]
        [JsonProperty("time")]
        public uint Timestamp;

        [BinaryProperty(7)]
        [JsonProperty("index")]
        public uint Index;

        [BinaryProperty(8)]
        [JsonProperty("nonce")]
        public ulong ConsensusData;

        [BinaryProperty(9)]
        [JsonProperty("nextconsensus")]
        public UInt160 NextConsensus;

        [BinaryProperty(10)]
        [JsonProperty("nextblockhash")]
        public UInt256 NextBlockHash;

        [BinaryProperty(12)]
        [JsonProperty("script")]
        public Witness Script;

        [BinaryProperty(13)]
        [JsonProperty("confirmations")]
        public int Confirmations;

        [JsonProperty("txcount")]
        public int TransactionCount => TransactionHashes?.Length ?? 0;

        [BinaryProperty(14)]
        [JsonProperty("txhashes")]
        public string[] TransactionHashes { get; set; }
    }
}
