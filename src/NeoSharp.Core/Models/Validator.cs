using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Types;
using Newtonsoft.Json;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Validator
    {
        [BinaryProperty(1)]
        [JsonProperty("publickey")]
        public ECPoint PublicKey;

        [BinaryProperty(2)]
        [JsonProperty("registered")]
        public bool Registered;

        [BinaryProperty(3)]
        [JsonProperty("votes")]
        public Fixed8 Votes;

        public Validator()
        {
        }

        public Validator(ECPoint publicKey)
        {
            PublicKey = publicKey;
            Registered = false;
            Votes = Fixed8.Zero;
        }
    }
}