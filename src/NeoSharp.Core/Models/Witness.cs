using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Witness : WithHash160, IEquatable<Witness>
    {
        [BinaryProperty(0, MaxLength = 65536)]
        [JsonProperty("invocation")]
        public byte[] InvocationScript;

        [BinaryProperty(1, MaxLength = 65536)]
        [JsonProperty("verification")]
        public byte[] VerificationScript;

        /// <summary>
        /// Check if is equal to other
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if equal</returns>
        public bool Equals(Witness other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return InvocationScript.SequenceEqual(other.InvocationScript) && VerificationScript.SequenceEqual(other.VerificationScript);
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

            if (!(obj is Witness other)) return false;

            return InvocationScript.SequenceEqual(other.InvocationScript) && VerificationScript.SequenceEqual(other.VerificationScript);
        }

        /// <summary>
        /// Get HashCode
        /// </summary>
        /// <returns>Return hashcode</returns>
        public override int GetHashCode()
        {
            return BitConverter.ToInt32(InvocationScript, 0) + BitConverter.ToInt32(VerificationScript, 0);
        }

        public override byte[] GetHashData(IBinarySerializer serializer)
        {
            return VerificationScript;
        }
    }
}