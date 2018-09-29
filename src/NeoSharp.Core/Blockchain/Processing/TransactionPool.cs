using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public partial class TransactionPool : ITransactionPool
    {
        private readonly ITransactionOperationsManager _transactionOperationsManager;

        private const int DefaultCapacity = 50_000;

        private readonly ConcurrentDictionary<UInt256, TimeStampedTransaction> _transactionPool = new ConcurrentDictionary<UInt256, TimeStampedTransaction>();
        private readonly IComparer<TimeStampedTransaction> _comparer;

        public TransactionPool(ITransactionOperationsManager transactionOperationsManager, IComparer<Transaction> comparer = null)
        {
            _transactionOperationsManager = transactionOperationsManager;
            _comparer = new TimeStampedTransactionComparer(comparer);
        }

        public int Size => _transactionPool.Count;

        public int Capacity => DefaultCapacity;

        public void Add(Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            this._transactionOperationsManager.Sign(transaction);

            if (!this._transactionOperationsManager.Verify(transaction))
            {
                throw new InvalidOperationException($"The transaction with hash \"{transaction.Hash}\" was not passed verification.");
            }
            
            if (this.Where(p => p != transaction)
                .SelectMany(p => p.Inputs)
                .Intersect(transaction.Inputs)
                .Any())
            {
                throw new InvalidOperationException($"The transaction with hash \"{transaction.Hash}\" was already queued to be added.");
            }

            if (!_transactionPool.TryAdd(transaction.Hash, new TimeStampedTransaction(transaction)))
            {
                throw new InvalidOperationException($"The transaction with hash \"{transaction.Hash}\" was already queued to be added.");
            }
        }

        public void Remove(UInt256 hash)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            if (hash == UInt256.Zero) throw new ArgumentException(nameof(hash));

            _transactionPool.TryRemove(hash, out _);
        }

        public bool Contains(UInt256 hash)
        {
            if (hash == null) throw new ArgumentNullException(nameof(hash));
            if (hash == UInt256.Zero) throw new ArgumentException(nameof(hash));

            return _transactionPool.ContainsKey(hash);
        }

        public IEnumerator<Transaction> GetEnumerator()
        {
            return GetTransactions().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<Transaction> GetTransactions()
        {
            return _transactionPool
                .Values
                .OrderBy(_ => _, _comparer)
                .Select(tst => tst.Transaction);
        }
    }
}