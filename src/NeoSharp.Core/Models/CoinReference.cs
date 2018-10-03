using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class CoinReference : IEquatable<CoinReference>
    {
        [BinaryProperty(0)]
        [JsonProperty("txid")]
        public UInt256 PrevHash;

        [BinaryProperty(1)]
        [JsonProperty("vout")]
        public ushort PrevIndex;

        /// <summary>
        /// Check if is equal to other
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if equal</returns>
        public bool Equals(CoinReference other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return PrevHash.Equals(other.PrevHash) && PrevIndex.Equals(other.PrevIndex);
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

            if (!(obj is CoinReference cx)) return false;

            return PrevHash.Equals(cx.PrevHash) && PrevIndex.Equals(cx.PrevIndex);
        }

        /// <summary>
        /// Get HashCode
        /// </summary>
        /// <returns>Return hashcode</returns>
        public override int GetHashCode()
        {
            return PrevHash.GetHashCode() + PrevIndex.GetHashCode();
        }
    }
}