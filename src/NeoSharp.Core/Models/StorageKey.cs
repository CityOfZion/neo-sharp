using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Cryptography;
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
            if (obj.GetType() != GetType()) return false;

            return Equals((StorageKey)obj);
        }

        public override int GetHashCode()
        {
            return ScriptHash.GetHashCode() + (int)Crypto.Default.Murmur32(Key, 0);
        }
    }
}