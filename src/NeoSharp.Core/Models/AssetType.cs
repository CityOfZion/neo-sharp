using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace NeoSharp.Core.Models
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum AssetType : byte
    {
        CreditFlag = 0x40,
        DutyFlag = 0x80,

        GoverningToken = 0x00,
        UtilityToken = 0x01,
        Currency = 0x08,
        Share = DutyFlag | 0x10,
        Invoice = DutyFlag | 0x18,
        Token = CreditFlag | 0x20,
    }
}
