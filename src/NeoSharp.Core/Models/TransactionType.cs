using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoSharp.Core.Models
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionType : byte
    {
        MinerTransaction = 0x00,
        IssueTransaction = 0x01,
        ClaimTransaction = 0x02,
        EnrollmentTransaction = 0x20,
        RegisterTransaction = 0x40,
        ContractTransaction = 0x80,
        StateTransaction = 0x90,
        PublishTransaction = 0xd0,
        InvocationTransaction = 0xd1
    }
}
