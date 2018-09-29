using System;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing
{
    public partial class TransactionPool
    {
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
    }
}