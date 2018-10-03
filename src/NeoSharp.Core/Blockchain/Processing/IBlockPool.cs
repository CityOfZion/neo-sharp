using System;
using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public interface IBlockPool
    {
        int Size { get; }

        int Capacity { get; }

        event EventHandler<Block> OnAdded;

        bool TryGet(uint height, out Block block);

        void Add(Block block);

        bool Contains(UInt256 hash);

        void Remove(uint height);
    }
}