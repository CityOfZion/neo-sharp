using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Types
{
    public interface ITransactionContext
    {
        Fixed8 DefaultSystemFee { get; }
        UInt256 UtilityTokenHash { get; }
        UInt256 GoverningTokenHash { get; }
        Fixed8 GetSystemFee(Transaction tx);
    }
}