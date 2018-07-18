using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NeoSharp.Core.Models
{
    [Flags]
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CoinState : byte
    {
        New = 0x00,
        Confirmed = 1 << 0,
        Spent = 1 << 1,
        Claimed = 1 << 3,
        Locked = 1 << 4,
        Frozen = 1 << 5,
    }
}