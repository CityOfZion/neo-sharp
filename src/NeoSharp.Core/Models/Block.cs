using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Block
    {
        [BinaryProperty(1)]
        [JsonProperty("hash")]
        public string Hash;

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
        public int Index;

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
        public int TxCount => TxHashes?.Length ?? 0;

        [BinaryProperty(14)]
        [JsonProperty("txhashes")]
        public string[] TxHashes { get; set; }

        public string ToJson(bool indent = false)
        {
            return JsonConvert.SerializeObject(this, indent ? Formatting.Indented : Formatting.None);
        }
    }
}
