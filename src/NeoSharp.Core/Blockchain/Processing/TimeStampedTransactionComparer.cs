using System;
using System.Collections.Generic;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing
{
    public partial class TransactionPool
    {
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
    }
}