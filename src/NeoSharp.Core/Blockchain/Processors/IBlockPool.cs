using System;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Processors
{
    public interface IBlockPool
    {
        Block CurrentBlock { get; set; }

        int Size { get; }

        int Capacity { get; }

        event EventHandler<Block> OnAdded;

        bool TryGet(uint index, out Block block);

        void Add(Block block);

        bool Contains(UInt256 blockHash);

        void Remove(uint index);
    }
}