using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.State
{
    public interface IAccountManager
    {
        Task<Account> Get(UInt160 hash);
        Task UpdateBalance(UInt160 hash, UInt256 assetId, Fixed8 delta);
    }
}