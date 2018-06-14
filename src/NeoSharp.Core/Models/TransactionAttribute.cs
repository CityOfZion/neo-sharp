using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    [BinaryTypeSerializer(typeof(TransactionAttributeConverter))]
    public class TransactionAttribute : IEquatable<TransactionAttribute>
    {
        [BinaryProperty(0)]
        [JsonProperty("usage")]
        public TransactionAttributeUsage Usage;

        [BinaryProperty(1)]
        [JsonProperty("data")]
        public byte[] Data;

        /// <summary>
        /// Check if is equal to other
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if equal</returns>
        public bool Equals(TransactionAttribute other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return Data.SequenceEqual(other.Data) && Usage == other.Usage;
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

            if (!(obj is TransactionAttribute other)) return false;

            return Data.SequenceEqual(other.Data) && Usage == other.Usage;
        }

        /// <summary>
        /// Get HashCode
        /// </summary>
        /// <returns>Return hashcode</returns>
        public override int GetHashCode()
        {
            var l = Data.Length;
            if (l < 4) return l;

            return BitConverter.ToInt32(Data, 0);
        }
    }
}