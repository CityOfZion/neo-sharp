using System;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public class TransactionOperationsManager : ITransactionOperationsManager
    {
        #region Private Fields 
        private readonly Crypto _crypto;
        private readonly IWitnessOperationsManager _witnessOperationsManager;
        private readonly ITransactionPool _transactionPool;
        private readonly IBlockchain _blockchain;
        private readonly ITransactionContext _transactionContext;
        #endregion

        #region Constructor 
        public TransactionOperationsManager(Crypto crypto, IWitnessOperationsManager witnessOperationsManager, 
            ITransactionPool transactionPool, IBlockchain blockchain, ITransactionContext transactionContext)
        {
            _crypto = crypto;
            _witnessOperationsManager = witnessOperationsManager;
            _transactionPool = transactionPool;
            _blockchain = blockchain;
            _transactionContext = transactionContext;
        }
        #endregion

        #region ITransactionOperationsManager implementation 
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

        public bool Verify(Transaction transaction)
        {
            if (transaction.Attributes.Any(p =>
                p.Usage == TransactionAttributeUsage.ECDH02 || p.Usage == TransactionAttributeUsage.ECDH03))
            {
                return false;
            }
            
            for (var i = 1; i < transaction.Inputs.Length; i++)
            {
                for (var j = 0; j < i; j++)
                {
                    if (transaction.Inputs[i].PrevHash == transaction.Inputs[j].PrevHash
                        && transaction.Inputs[i].PrevIndex == transaction.Inputs[j].PrevIndex)
                    {
                        return false;
                    }
                }
            }

            if (_transactionPool
                .Where(p => p != transaction)
                .SelectMany(p => p.Inputs)
                .Intersect(transaction.Inputs)
                .Any())
            {
                return false;
            }

            if (_blockchain.IsDoubleSpend(transaction))
            {
                return false;
            }

            foreach (var group in transaction.Outputs.GroupBy(p => p.AssetId))
            {
                var asset = _blockchain.GetAsset(group.Key).Result;
                
                if (asset == null)
                {
                    return false;
                }
                
                // TODO: Should we check for `asset.Expiration <= _blockchain.Height + 1` ??
                if (asset.AssetType != AssetType.GoverningToken 
                    && asset.AssetType != AssetType.UtilityToken)
                {
                    return false;
                }
                
                var tenPoweredToEightMinusAssetPrecision = (long) Math.Pow(10, 8 - asset.Precision);

                if (group.Any(output => output.Value.Value % tenPoweredToEightMinusAssetPrecision != 0))
                {
                    return false;
                }
            }

            var results = GetTransactionResults(transaction)?.ToArray();

            if (results == null)
            {
                return false;
            }
            
            var resultsDestroy = results.Where(p => p.Amount > Fixed8.Zero).ToArray();
            
            if (resultsDestroy.Length > 1)
            {
                return false;
            }

            if (resultsDestroy.Length == 1 
                && resultsDestroy[0].AssetId != _transactionContext.UtilityTokenHash)
            {
                return false;
            }

            if (_transactionContext.SystemFee > Fixed8.Zero 
                && (resultsDestroy.Length == 0 
                    || resultsDestroy[0].Amount < _transactionContext.SystemFee))
            {
                return false;
            }
            
            var resultsIssue = results.Where(p => p.Amount < Fixed8.Zero).ToArray();
            
            if (resultsIssue.Any(p => p.AssetId != _transactionContext.UtilityTokenHash)
                && (transaction.Type == TransactionType.ClaimTransaction
                    || transaction.Type == TransactionType.IssueTransaction))
            {
                return false;
            }
            
            if (transaction.Type != TransactionType.MinerTransaction 
                && resultsIssue.Length > 0)
            {
                return false;
            }
            
            // TODO: Verify Receiving Scripts?

            if (transaction.Witness.Any(witness => !_witnessOperationsManager.Verify(witness)))
            {
                return false;
            }

            return true;
        }

        private IEnumerable<TransactionResult> GetTransactionResults(Transaction transaction)
        {
            return GetReferences(transaction)?.Values.Select(p => new
            {
                p.AssetId,
                p.Value
            }).Concat(transaction.Outputs.Select(p => new
            {
                p.AssetId,
                Value = -p.Value
            })).GroupBy(p => p.AssetId, (k, g) => new TransactionResult
            {
                AssetId = k,
                Amount = g.Sum(p => p.Value)
            }).Where(p => p.Amount != Fixed8.Zero);
        }
        
        private class TransactionResult
        {
            public UInt256 AssetId;
            public Fixed8 Amount;
        }

        private Dictionary<CoinReference, TransactionOutput> GetReferences(Transaction transaction)
        {
            var references = new Dictionary<CoinReference, TransactionOutput>();
            
            foreach (var group in transaction.Inputs.GroupBy(p => p.PrevHash))
            {
                var tx = _blockchain.GetTransaction(group.Key).Result;
                
                if (tx == null)
                {
                    references = null;
                    break;
                }

                foreach (var p in group)
                {
                    if (tx.Outputs.Length > p.PrevIndex)
                    {
                        references.Add(p, tx.Outputs[p.PrevIndex]);
                    }
                }
            }

            return references;
        }
        
        #endregion
    }
}