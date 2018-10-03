using System.Threading.Tasks;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.State
{
    public interface IAccountManager
    {
        Task<Account> Get(UInt160 hash);
        Task UpdateBalance(UInt160 hash, UInt256 assetId, Fixed8 delta);
        Task UpdateVotes(UInt160 hash, ECPoint[] newCandidates);
    }
}