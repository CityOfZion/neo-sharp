using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence.Contexts
{
    public interface ITransactionContext
    {
        Task Add(Transaction transaction);

        Task<Transaction> GetTransactionByHash(UInt256 transactionHash);

        Task<IEnumerable<Transaction>> GetTransactionsForBlock(UInt256 blockHeaderHash);
    }
}
