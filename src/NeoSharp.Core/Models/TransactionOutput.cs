using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class TransactionOutput : IEquatable<TransactionOutput>
    {
        [BinaryProperty(0)]
        [JsonProperty("asset")]
        public UInt256 AssetId;

        [BinaryProperty(1)]
        [JsonProperty("value")]
        public Fixed8 Value;

        [BinaryProperty(2)]
        [JsonProperty("address")]
        public UInt160 ScriptHash;

        /// <summary>
        /// Check if is equal to other
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if equal</returns>
        public bool Equals(TransactionOutput other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return AssetId.Equals(other.AssetId) && Value.Equals(other.Value) && ScriptHash.Equals(other.ScriptHash);
        }

        /// <summary>
        /// Check if is equal to other
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if equal</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null) return false;

            if (!(obj is TransactionOutput other)) return false;

            return AssetId.Equals(other.AssetId) && Value.Equals(other.Value) && ScriptHash.Equals(other.ScriptHash);
        }

        /// <summary>
        /// Get HashCode
        /// </summary>
        /// <returns>Return hashcode</returns>
        public override int GetHashCode()
        {
            return BitConverter.ToInt32(AssetId.ToArray(), 0);
        }
    }
}