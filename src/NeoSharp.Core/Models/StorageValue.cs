using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class StorageValue : ICloneable<StorageValue>
    {
        [BinaryProperty(1)]
        [JsonProperty("value")]
        public byte[] Value;

        public StorageValue Clone()
        {
            return new StorageValue
            {
                Value = Value
            };
        }

        public void FromReplica(StorageValue replica)
        {
            Value = replica.Value;
        }
    }
}