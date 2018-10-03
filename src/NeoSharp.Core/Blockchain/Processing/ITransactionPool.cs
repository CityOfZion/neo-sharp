using System.Collections.Generic;
using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public interface ITransactionPool : IEnumerable<Transaction>
    {
        int Size { get; }

        int Capacity { get; }

        void Add(Transaction transaction);

        void Remove(UInt256 hash);

        bool Contains(UInt256 hash);
    }
}