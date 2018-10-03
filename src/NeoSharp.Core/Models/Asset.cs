using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    public class Asset
    {
        [BinaryProperty(1)]
        [JsonProperty("hash")]
        public UInt256 Id;

        [BinaryProperty(2)]
        [JsonProperty("type")]
        public AssetType AssetType;

#if !DEBUG //TODO #371: Chinese characters are having an issue in the VS Debugger?  We can't serialize Chinese Chars?
        [BinaryProperty(3)]
        [JsonProperty("name")]
#endif
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

    }
}
