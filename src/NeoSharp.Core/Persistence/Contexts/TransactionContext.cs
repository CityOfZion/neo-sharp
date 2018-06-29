using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence.Contexts
{
    public class TransactionContext : ITransactionContext
    {

        #region Private Fields 
        private readonly IDbModel _model;
        private readonly ICrypto _crypto;
        private readonly IBinarySerializer _serializer;

        #endregion

        #region Constructor 
        public TransactionContext(IDbModel model, ICrypto crypto, IBinarySerializer serializer)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        #endregion

        #region ITransactionContext Implementation
        public Task Add(Transaction transaction)
        {
            transaction.Hash = new UInt256(_crypto.Hash256(GetHashData(transaction)));

            return _model.Create(DataEntryPrefix.DataTransaction, transaction.Hash, transaction);
        }

        public Task<Transaction> GetTransactionByHash(UInt256 transactionHash)
        {
            return _model.Get<Transaction>(DataEntryPrefix.DataTransaction, transactionHash);
        }

        public Task<IEnumerable<Transaction>> GetTransactionsForBlock(UInt256 blockHeaderHash)
        {
            // TODO [AboimPinto]: this method should not be on the TransactionContext or BlockHeaderContext because need to be in a place that have access to both contexts9

            throw new System.NotImplementedException();
        }
        #endregion

        /// <summary>
        /// Get hash data
        /// </summary>
        /// <returns>Return hash data</returns>
        public byte[] GetHashData(Transaction transaction)
        {
            // Exclude signature

            return _serializer.Serialize(this, new BinarySerializerSettings
            {
                Filter = (name => name != nameof(transaction.Scripts))
            });
        }
    }
}
