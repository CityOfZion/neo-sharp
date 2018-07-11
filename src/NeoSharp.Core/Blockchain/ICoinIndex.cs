using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain
{
    public interface ICoinIndex
    {
        Task<HashSet<CoinReference>> GetSpendable(UInt160 hash);
        Task<HashSet<CoinReference>> GetClaimable(UInt160 hash);
        Task<uint> GetHeight();
        Task IndexBlock(Block block);
    }
}