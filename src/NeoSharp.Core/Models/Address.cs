using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Address
    {
        [BinaryProperty(1)]
        [JsonProperty("id")]
        public string Id;

        [BinaryProperty(2)]
        [JsonProperty("transactions")]
        public Transaction[] Transactions = new Transaction[0];

        [BinaryProperty(3)]
        [JsonProperty("coins")]
        public AddressCoin[] Coins = new AddressCoin[0];

        [BinaryProperty(4)]
        [JsonProperty("firsttimestamp")]
        public int FirstTimeStamp;

        [BinaryProperty(5)]
        [JsonProperty("lasttimestamp")]
        public int LastTimeStamp;
    }
}