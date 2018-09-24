using System;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public class TransactionSigner : ITransactionSigner
    {
        #region Private Fields 
        private readonly Crypto _crypto;
        private readonly IWitnessOperationsManager _witnessOperationsManager;
        #endregion

        #region Constructor 
        public TransactionSigner(Crypto crypto, IWitnessOperationsManager witnessOperationsManager)
        {
            _crypto = crypto;
            _witnessOperationsManager = witnessOperationsManager;
        }
        #endregion

         
        public void Sign(Transaction transaction)
        {
            transaction.Hash = new UInt256(_crypto.Hash256(BinarySerializer.Default.Serialize(transaction, new BinarySerializerSettings
            {
                Filter = a => a != nameof(transaction.Witness)
            })));

            if (transaction.Witness == null) return;
            foreach (var witness in transaction.Witness)
            {
                _witnessOperationsManager.Sign(witness);
            }
        }
        
    }
}