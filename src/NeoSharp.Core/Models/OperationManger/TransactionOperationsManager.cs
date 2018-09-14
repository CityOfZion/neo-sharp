using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public class TransactionOperationsManager : ITransactionOperationsManager
    {
        #region Private Fields 
        private readonly Crypto _crypto;
        private readonly IWitnessOperationsManager _witnessOperationsManager;
        #endregion

        #region Constructor 
        public TransactionOperationsManager(Crypto crypto, IWitnessOperationsManager witnessOperationsManager)
        {
            this._crypto = crypto;
            this._witnessOperationsManager = witnessOperationsManager;
        }
        #endregion

        #region ITransactionOperationsManager implementation 
        public void Sign(Transaction transaction)
        {
            transaction.Hash = new UInt256(this._crypto.Hash256(BinarySerializer.Default.Serialize(transaction, new BinarySerializerSettings
            {
                Filter = a => a != nameof(transaction.Witness)
            })));

            if (transaction.Witness == null) return;
            foreach (var witness in transaction.Witness)
            {
                this._witnessOperationsManager.Sign(witness);
            }
        }

        public bool Verify(Transaction transaction)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}