using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Witness : IEquatable<Witness>
    {
        [JsonProperty("hash")]
        public UInt160 Hash { get; set; }

        [BinaryProperty(0, MaxLength = 65536)]
        [JsonProperty("invocation")]
        public byte[] InvocationScript { get; set; }

        [BinaryProperty(1, MaxLength = 65536)]
        [JsonProperty("verification")]
        public byte[] VerificationScript { get; set; }

        /// <inheritdoc />
        public bool Equals(Witness other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return InvocationScript.SequenceEqual(other.InvocationScript) && VerificationScript.SequenceEqual(other.VerificationScript);
        }

        /// <summary>
        /// Check if is equal to other
        /// </summary>
        /// <param name="obj">Other</param>
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
            var l = 0;

            if (InvocationScript.Length >= 4)
            {
                l += BitConverter.ToInt32(InvocationScript, 0);
            }
            else
            {
                l += InvocationScript.Length;
            }

            if (VerificationScript.Length >= 4)
            {
                l += BitConverter.ToInt32(VerificationScript, 0);
            }
            else
            {
                l += VerificationScript.Length;
            }

            return l;
        }
    }
}