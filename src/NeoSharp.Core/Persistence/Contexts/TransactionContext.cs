using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence.Contexts
{
    public class TransactionContext : ITransactionContext
    {

        #region Private Fields 
        private readonly IDbModel _model;
        #endregion

        #region Constructor 
        public TransactionContext(IDbModel model)
        {
            _model = model;
        }
        #endregion

        #region ITransactionContext Implementation
        public Task Add(Transaction transaction)
        {
            return _model.Create(transaction, DataEntryPrefix.DataTransaction);
        }

        public Task<Transaction> GetTransactionByHash(UInt256 transactionHash)
        {
            return _model.GetByHash<Transaction>(transactionHash, DataEntryPrefix.DataTransaction);
        }

        public Task<IEnumerable<Transaction>> GetTransactionsForBlock(UInt256 blockHeaderHash)
        {
            // TODO [AboimPinto]: this method should not be on the TransactionContext or BlockHeaderContext because need to be in a place that have access to both contexts9

            throw new System.NotImplementedException();
        }
        #endregion
    }
}
