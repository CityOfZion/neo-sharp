using System;
using System.Threading.Tasks;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Blockchain.Processors
{
    public interface IBlockProcessor : IDisposable
    {
        event Func<Block, Task> OnBlockProcessed;

        void Run(Block currentBlock);

        /// <summary>
        /// Add the specified block to the blockchain
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        Task AddBlock(Block block);
    }
}