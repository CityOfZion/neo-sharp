using System;

namespace NeoSharp.Core.Models
{
    [Flags]
    public enum ContractMetadata : byte
    {
        NoProperty = 0,
        HasStorage = 1 << 0,
        HasDynamicInvoke = 1 << 1,
        Payable = 1 << 2
    }
}