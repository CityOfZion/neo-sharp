using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class StorageKey : IEquatable<StorageKey>
    {
        [BinaryProperty(1)]
        [JsonProperty("scripthash")]
        public UInt160 ScriptHash;

        [BinaryProperty(2)]
        [JsonProperty("key")]
        public byte[] Key;

        public bool Equals(StorageKey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(ScriptHash, other.ScriptHash) && Key.SequenceEqual(other.Key);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((StorageKey) obj);
        }

        public override int GetHashCode()
        {
            // TODO: return ScriptHash.GetHashCode() + (int)Key.Murmur32(0); in NEO
            unchecked
            {
                return ((ScriptHash != null ? ScriptHash.GetHashCode() : 0) * 397) ^ (Key != null ? Key.GetHashCode() : 0);
            }
        }
    }
}