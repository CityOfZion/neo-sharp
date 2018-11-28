using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        #region Private Fields 
        private readonly IRepository _repository;
        #endregion

        #region Construtor 
        public TransactionRepository(IRepository repository)
        {
            _repository = repository;
        }
        #endregion

        #region ITransactionModel Implementation 

        /// <inheritdoc />
        public async Task<Transaction> GetTransaction(UInt256 hash)
        {
            return await _repository.GetTransaction(hash);
        }

        public async Task<bool> ContainsTransaction(UInt256 hash)
        {
            // TODO #389: Optimize this
            return await GetTransaction(hash) != null;
        }

        public async Task<IEnumerable<Transaction>> GetTransactions(IReadOnlyCollection<UInt256> transactionHashes)
        {
            var transactions = new List<Transaction>();

            foreach (var hash in transactionHashes)
            {
                var transaction = await GetTransaction(hash);

                if (transaction == null) continue;
                transactions.Add(transaction);
            }

            return transactions;
        }

        /// <inheritdoc />
        public bool IsDoubleSpend(Transaction transaction)
        {
            if (transaction.Inputs.Length == 0)
            {
                return false;
            }

            foreach (var group in transaction.Inputs.GroupBy(p => p.PrevHash))
            {
                var states = _repository.GetCoinStates(group.Key).Result;

                if (states == null || group.Any(p => p.PrevIndex >= states.Length || states[p.PrevIndex].HasFlag(CoinState.Spent)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public TransactionOutput GetUnspent(UInt256 hash, ushort index)
        {
            var states = _repository.GetCoinStates(hash).Result;

            if (states == null || index >= states.Length || states[index].HasFlag(CoinState.Spent))
            {
                return null;
            }

            return GetTransaction(hash).Result.Outputs[index];
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TransactionOutput>> GetUnspent(UInt256 hash)
        {
            var outputs = new List<TransactionOutput>();

            var states = await _repository.GetCoinStates(hash);
            if (states != null)
            {
                var tx = await GetTransaction(hash);
                for (var i = 0; i < states.Length; i++)
                {
                    if (!states[i].HasFlag(CoinState.Spent))
                    {
                        outputs.Add(tx.Outputs[i]);
                    }
                }
            }
            return outputs;
        }

        public async Task<IReadOnlyDictionary<CoinReference, TransactionOutput>> GetReferences(Transaction transaction)
        {
            var references = new Dictionary<CoinReference, TransactionOutput>();

            foreach (var group in transaction.Inputs.GroupBy(p => p.PrevHash))
            {
                var tx = await GetTransaction(group.Key);
                if (tx == null) return null;

                foreach (var reference in group.Select(p => new
                {
                    Input = p,
                    Output = tx.Outputs[p.PrevIndex]
                }))
                {
                    references.Add(reference.Input, reference.Output);
                }
            }

            return references;
        }

        #endregion
    }
}