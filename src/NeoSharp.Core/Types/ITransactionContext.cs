using NeoSharp.Core.Models;
using NeoSharp.Types;

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