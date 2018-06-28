using System;
using NeoSharp.BinarySerialization;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Coin : IEquatable<Coin>
    {
        private string _address = null;

        [BinaryProperty(0)]
        [JsonProperty("reference")]
        public CoinReference Reference;

        [BinaryProperty(1)]
        [JsonProperty("vout")]
        public TransactionOutput Output;

        [BinaryProperty(2)]
        [JsonProperty("state")]
        public CoinState State;

        [JsonProperty("address")]
        public string Address
        {
            get
            {
                if (_address == null)
                {
                    //_address = Wallet.ToAddress(Output.ScriptHash);
                }
                return _address;
            }
        }

        public bool Equals(Coin other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return Reference.Equals(other.Reference);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Coin other)) return false;

            return Reference.Equals(other.Reference);
        }

        public override int GetHashCode()
        {
            return Reference.GetHashCode();
        }
    }
}