using System.Collections.Generic;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processing
{
    public interface IBlockPool : IEnumerable<Block>
    {
        int Size { get; }

        int Capacity { get; }

        bool TryGet(uint height, out Block block);

        bool TryAdd(Block block);

        bool TryRemove(uint height);
    }
}