using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    public class Asset : ICloneable<Asset>
    {
        [BinaryProperty(1)]
        [JsonProperty("hash")]
        public UInt256 Id;

        [BinaryProperty(2)]
        [JsonProperty("type")]
        public AssetType AssetType;

        [BinaryProperty(3)]
        [JsonProperty("name")]
        public string Name;

        [BinaryProperty(4)]
        [JsonProperty("amount")]
        public Fixed8 Amount;

        [BinaryProperty(5)]
        [JsonProperty("available")]
        public Fixed8 Available;

        [BinaryProperty(6)]
        [JsonProperty("precision")]
        public byte Precision;

        [BinaryProperty(7)]
        [JsonProperty("owner")]
        public ECPoint Owner;

        [BinaryProperty(8)]
        [JsonProperty("admin")]
        public UInt160 Admin;

        [BinaryProperty(9)]
        [JsonProperty("issuer")]
        public UInt160 Issuer;

        [BinaryProperty(10)]
        [JsonProperty("expiration")]
        public uint Expiration;

        [BinaryProperty(11)]
        [JsonProperty("isFrozen")]
        public bool IsFrozen;

        public Asset Clone()
        {
            return new Asset
            {
                Id = Id,
                AssetType = AssetType,
                Name = Name,
                Amount = Amount,
                Available = Available,
                Precision = Precision,
                Owner = Owner,
                Admin = Admin,
                Issuer = Issuer,
                Expiration = Expiration,
                IsFrozen = IsFrozen
            };
        }

        public void FromReplica(Asset replica)
        {
            Id = replica.Id;
            AssetType = replica.AssetType;
            Name = replica.Name;
            Amount = replica.Amount;
            Available = replica.Available;
            Precision = replica.Precision;
            Owner = replica.Owner;
            Admin = replica.Admin;
            Issuer = replica.Issuer;
            Expiration = replica.Expiration;
            IsFrozen = replica.IsFrozen;
        }
    }
}
