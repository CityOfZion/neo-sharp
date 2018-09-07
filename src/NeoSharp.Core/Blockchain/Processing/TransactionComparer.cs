using System.Collections.Generic;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class TransactionComparer : IComparer<Transaction>
    {
        public int Compare(Transaction x, Transaction y)
        {
            // TODO: x.NetworkFee.CompareTo(y.NetworkFee)
            return 0;
        }
    }
}