using System;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Blockchain.Processors
{
    public interface IBlockProcessor : IDisposable
    {
        int BlockPoolSize { get; }

        int BlockPoolCapacity { get; }

        event Func<Block, Task> OnBlockProcessed;

        void Run(Block currentBlock);

        /// <summary>
        /// Add the specified block to the blockchain
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        Task AddBlock(Block block);

        /// <summary>
        /// Determine whether the specified block is contained in the blockchain
        /// </summary>
        /// <param name="blockHash"></param>
        /// <returns></returns>
        Task<bool> ContainsBlock(UInt256 blockHash);
    }
}