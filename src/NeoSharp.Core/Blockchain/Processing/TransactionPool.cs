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
    public class TransactionPool : ITransactionPool
    {
        private readonly ITransactionSigner _transactionSigner;
        private readonly ITransactionVerifier _transactionVerifier;

        private class TimeStampedTransaction
        {
            public Transaction Transaction { get; }

            public DateTime CreatedAt { get; }

            public TimeStampedTransaction(Transaction transaction)
            {
                Transaction = transaction;
                CreatedAt = DateTime.UtcNow;
            }
        }

        private class TimeStampedTransactionComparer : IComparer<TimeStampedTransaction>
        {
            private readonly IComparer<Transaction> _comparer;
            private readonly Comparer<DateTime> _createdAtComparer = Comparer<DateTime>.Default;

            public TimeStampedTransactionComparer(IComparer<Transaction> comparer = null)
            {
                _comparer = comparer ?? Comparer<Transaction>.Default;
            }

            public int Compare(TimeStampedTransaction a, TimeStampedTransaction b)
            {
                if (a == b) return 0;
                if (a == null) return -1;
                if (b == null) return 1;

                var transactionComparisonResult = _comparer.Compare(a.Transaction, b.Transaction);

                return transactionComparisonResult == 0
                    ? _createdAtComparer.Compare(a.CreatedAt, b.CreatedAt)
                    : transactionComparisonResult;
            }
        }

        private const int DefaultCapacity = 50_000;

        private readonly ConcurrentDictionary<UInt256, TimeStampedTransaction> _transactionPool = new ConcurrentDictionary<UInt256, TimeStampedTransaction>();
        private readonly IComparer<TimeStampedTransaction> _comparer;

        public TransactionPool(ITransactionSigner transactionSigner, ITransactionVerifier transactionVerifier, IComparer<Transaction> comparer = null)
        {
            _transactionSigner = transactionSigner;
            _transactionVerifier = transactionVerifier;
            _comparer = new TimeStampedTransactionComparer(comparer);
        }

        public int Size => _transactionPool.Count;

        public int Capacity => DefaultCapacity;

        public void Add(Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            this._transactionSigner.Sign(transaction);

            if (!this._transactionVerifier.Verify(transaction))
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