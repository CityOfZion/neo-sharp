using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using NeoSharp.Cryptography;
using NeoSharp.Types;

namespace NeoSharp.Core.Models.OperationManager
{
    public class TransactionOperationManager : ITransactionOperationsManager
    {
        #region Private Fields 
        private readonly Crypto _crypto;
        private readonly IBinarySerializer _binarySerializer;
        private readonly IWitnessOperationsManager _witnessOperationsManager;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAssetRepository _assetRepository;
        private readonly ITransactionContext _transactionContext;
        #endregion

        #region Constructor 
        public TransactionOperationManager(
            Crypto crypto, 
            IBinarySerializer binarySerializer,
            IWitnessOperationsManager witnessOperationsManager, 
            ITransactionRepository transactionRepository,
            IAssetRepository assetRepository,
            ITransactionContext transactionContext)
        {
            _crypto = crypto;
            _binarySerializer = binarySerializer;
            _witnessOperationsManager = witnessOperationsManager;
            _transactionRepository = transactionRepository;
            _assetRepository = assetRepository;
            _transactionContext = transactionContext;
        }
        #endregion

        #region ITransactionOperationsManager implementation 
        public void Sign(Transaction transaction)
        {
            transaction.Hash = new UInt256(_crypto.Hash256(_binarySerializer.Serialize(transaction, new BinarySerializerSettings
            {
                Filter = a => a != nameof(transaction.Witness)
            })));

            if (transaction.Witness == null) return;

            foreach (var witness in transaction.Witness)
            {
                _witnessOperationsManager.Sign(witness);
            }
        }

        public async Task<bool> Verify(Transaction transaction)
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
                    if (transaction.Inputs[i].PrevHash == transaction.Inputs[j].PrevHash &&
                        transaction.Inputs[i].PrevIndex == transaction.Inputs[j].PrevIndex)
                    {
                        return false;
                    }
                }
            }

            if (_transactionRepository.IsDoubleSpend(transaction))
            {
                return false;
            }

            foreach (var group in transaction.Outputs.GroupBy(p => p.AssetId))
            {
                var asset = await _assetRepository.GetAsset(group.Key);
                if (asset == null)
                {
                    return false;
                }

                // TODO: Should we check for `asset.Expiration <= _blockchain.Height + 1` ??
                if (asset.AssetType != AssetType.GoverningToken &&
                    asset.AssetType != AssetType.UtilityToken)
                {
                    return false;
                }

                var tenPoweredToEightMinusAssetPrecision = (long)Math.Pow(10, 8 - asset.Precision);
                if (group.Any(output => output.Value.Value % tenPoweredToEightMinusAssetPrecision != 0))
                {
                    return false;
                }
            }

            var results = await GetTransactionResults(transaction);
            if (results.Length == 0)
            {
                return false;
            }

            var resultsDestroy = results.Where(p => p.Amount > Fixed8.Zero).ToArray();
            if (resultsDestroy.Length > 1)
            {
                return false;
            }

            if (resultsDestroy.Length == 1 &&
                resultsDestroy[0].AssetId != _transactionContext.UtilityTokenHash)
            {
                return false;
            }

            if (_transactionContext.GetSystemFee(transaction) > Fixed8.Zero &&
                (resultsDestroy.Length == 0 || resultsDestroy[0].Amount < _transactionContext.GetSystemFee(transaction)))
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

            var verifiedTransactionWitnesses = await Task.WhenAll(transaction.Witness.Select(witness => _witnessOperationsManager.Verify(witness)));
            if (verifiedTransactionWitnesses.Any(tw => !tw))
            {
                return false;
            }

            return true;
        }

        public async Task<UInt160[]> GetScriptHashes(Transaction transaction)
        {
            var references = await GetReferences(transaction);
            var hashes = new HashSet<UInt160>(transaction.Inputs.Select(p => references[p].ScriptHash));

            hashes.UnionWith(
                transaction.Attributes
                    .Where(p => p.Usage == TransactionAttributeUsage.Script)
                    .Select(p => new UInt160(p.Data)));

            foreach (var group in transaction.Outputs.GroupBy(p => p.AssetId))
            {
                var asset = await _assetRepository.GetAsset(group.Key);
                if (asset == null)
                {
                    throw new InvalidOperationException();
                }

                if (asset.AssetType.HasFlag(AssetType.DutyFlag))
                {
                    hashes.UnionWith(group.Select(p => p.ScriptHash));
                }
            }

            return hashes.OrderBy(p => p).ToArray();
        }

        #endregion

        #region Private Methods

        private async Task<TransactionResult[]> GetTransactionResults(Transaction transaction)
        {
            var references = await GetReferences(transaction);

            return references.Values
                .Select(p => new
                {
                    p.AssetId,
                    p.Value
                })
                .Concat(transaction.Outputs
                    .Select(p => new
                    {
                        p.AssetId,
                        Value = -p.Value
                    }))
                .GroupBy(
                    p => p.AssetId,
                    (k, g) => new TransactionResult
                    {
                        AssetId = k,
                        Amount = g.Sum(p => p.Value)
                    })
                .Where(p => p.Amount != Fixed8.Zero)
                .ToArray();
        }

        private async Task<IReadOnlyDictionary<CoinReference, TransactionOutput>> GetReferences(Transaction transaction)
        {
            var references = new Dictionary<CoinReference, TransactionOutput>();

            foreach (var group in transaction.Inputs.GroupBy(p => p.PrevHash))
            {
                var tx = await _transactionRepository.GetTransaction(group.Key);
                if (tx == null)
                {
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