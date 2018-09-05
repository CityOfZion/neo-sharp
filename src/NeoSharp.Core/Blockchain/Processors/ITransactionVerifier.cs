using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processors
{
    public interface ITransactionVerifier
    {
        Task<bool> Verify(Transaction transaction, IReadOnlyCollection<Transaction> transactionPool);
    }
}