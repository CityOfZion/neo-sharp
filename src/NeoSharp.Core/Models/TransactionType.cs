using System;
using NeoSharp.Core.Caching;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NeoSharp.Core.Models
{
    [Serializable]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TransactionType : byte
    {
        [ReflectionCache(typeof(MinerTransaction))]
        MinerTransaction = 0x00,

        [ReflectionCache(typeof(IssueTransaction))]
        IssueTransaction = 0x01,

        [ReflectionCache(typeof(ClaimTransaction))]
        ClaimTransaction = 0x02,

#pragma warning disable CS0612 // Type or member is obsolete
        [ReflectionCache(typeof(EnrollmentTransaction))]
#pragma warning restore CS0612 // Type or member is obsolete
        EnrollmentTransaction = 0x20,

#pragma warning disable CS0612 // Type or member is obsolete
        [ReflectionCache(typeof(RegisterTransaction))]
#pragma warning restore CS0612 // Type or member is obsolete
        RegisterTransaction = 0x40,

        [ReflectionCache(typeof(ContractTransaction))]
        ContractTransaction = 0x80,

        [ReflectionCache(typeof(StateTransaction))]
        StateTransaction = 0x90,

#pragma warning disable CS0612 // Type or member is obsolete
        [ReflectionCache(typeof(PublishTransaction))]
#pragma warning restore CS0612 // Type or member is obsolete
        PublishTransaction = 0xd0,

        [ReflectionCache(typeof(InvocationTransaction))]
        InvocationTransaction = 0xd1
    }
}