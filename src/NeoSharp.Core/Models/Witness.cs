using NeoSharp.BinarySerialization;
using Newtonsoft.Json;
using System;

namespace NeoSharp.Core.Models
{
    [Serializable]
    public class Witness
    {
        [BinaryProperty(1)]
        [JsonProperty("invocation")]
        public string InvocationScript;

        [BinaryProperty(2)]
        [JsonProperty("verification")]
        public string VerificationScript;
    }
}