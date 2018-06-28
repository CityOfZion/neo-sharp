using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace NeoSharp.Core.Models
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CoinState : byte
    {
        Confirmed = 1 << 0,
        Spent = 1 << 1,
        Claimed = 1 << 3,
        Locked = 1 << 4,
        Frozen = 1 << 5,
    }
}
