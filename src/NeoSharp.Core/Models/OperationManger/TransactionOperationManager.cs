using System;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using NeoSharp.Cryptography;
using NeoSharp.Types;

namespace NeoSharp.Core.Models.OperationManger
{
    public class TransactionOperationManager : ITransactionOperationsManager
    {
        #region Private Fields 
        private readonly Crypto _crypto;
        private readonly IBinarySerializer _binarySerializer;
        private readonly IWitnessOperationsManager _witnessOperationsManager;
        private readonly ITransactionRepository _transactionModel;
        private readonly IAssetRepository _assetModel;
        private readonly ITransactionContext _transactionContext;
        #endregion

        #region Constructor 
        public TransactionOperationManager(
            Crypto crypto, 
            IBinarySerializer binarySerializer,
            IWitnessOperationsManager witnessOperationsManager, 
            ITransactionRepository transactionModel,
            IAssetRepository assetModel,
            ITransactionContext transactionContext)
        {
            this._crypto = crypto;
            this._binarySerializer = binarySerializer;
            this._witnessOperationsManager = witnessOperationsManager;
            this._transactionModel = transactionModel;
            this._assetModel = assetModel;
            _transactionContext = transactionContext;
        }
        #endregion

        #region ITransactionOperationsManager implementation 
        public void Sign(Transaction transaction)
        {
            transaction.Hash = new UInt256(this._crypto.Hash256(this._binarySerializer.Serialize(transaction, new BinarySerializerSettings
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

            if (this._transactionModel.IsDoubleSpend(transaction))
            {
                return false;
            }

            foreach (var group in transaction.Outputs.GroupBy(p => p.AssetId))
            {
                var asset = this._assetModel.GetAsset(group.Key).Result;

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

                var tenPoweredToEightMinusAssetPrecision = (long)Math.Pow(10, 8 - asset.Precision);

                if (group.Any(output => output.Value.Value % tenPoweredToEightMinusAssetPrecision != 0))
                {
                    return false;
                }
            }

            var results = this.GetTransactionResults(transaction)?.ToArray();

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
                && resultsDestroy[0].AssetId != this._transactionContext.UtilityTokenHash)
            {
                return false;
            }

            if (this._transactionContext.GetSystemFee(transaction) > Fixed8.Zero
                && (resultsDestroy.Length == 0
                    || resultsDestroy[0].Amount < this._transactionContext.GetSystemFee(transaction)))
            {
                return false;
            }

            var resultsIssue = results.Where(p => p.Amount < Fixed8.Zero).ToArray();

            if (resultsIssue.Any(p => p.AssetId != this._transactionContext.UtilityTokenHash)
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
        #endregion

        #region Private Methods
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

        private Dictionary<CoinReference, TransactionOutput> GetReferences(Transaction transaction)
        {
            var references = new Dictionary<CoinReference, TransactionOutput>();

            foreach (var group in transaction.Inputs.GroupBy(p => p.PrevHash))
            {
                var tx = this._transactionModel.GetTransaction(group.Key).Result;

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