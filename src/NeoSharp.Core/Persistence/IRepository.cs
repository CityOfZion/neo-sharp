using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence
{
    public interface IRepository
    {
        #region Blocks
        /// <summary>
        /// Adds a block header to the repository storage
        /// </summary>
        /// <param name="blockHeader"></param>
        Task AddBlockHeader(BlockHeaderBase blockHeader);

        /// <summary>
        /// Retrieves a hash by height / index
        /// </summary>
        /// <param name="height">The block height / index to retrieve</param>
        /// <returns>Block hash at specified height / index</returns>
        Task<UInt256> GetBlockHashFromHeight(uint height);

        /// <summary>
        /// Retrieves a block header by hash
        /// </summary>
        /// <param name="hash">Block id / hash</param>
        /// <returns>Block header with specified id</returns>
        Task<BlockHeader> GetBlockHeader(UInt256 hash);

        /// <summary>
        /// Retrieves the total / current block height
        /// </summary>
        /// <returns>Total / current block height</returns>
        Task<uint> GetTotalBlockHeight();

        /// <summary>
        /// Set the total/ current block height
        /// </summary>
        /// <param name="height">Total / current block height</param>
        Task SetTotalBlockHeight(uint height);
        #endregion

        #region Transactions
        /// <summary>
        /// Adds a transaction to the repository
        /// </summary>
        /// <param name="transaction">Transaction to add</param>
        Task AddTransaction(Transaction transaction);

        /// <summary>
        /// Retrieves a transaction by identifier / hash
        /// </summary>
        /// <param name="hash">Identifier / hash of the transaction</param>
        /// <returns>Transaction with the specified id / hash</returns>
        Task<Transaction> GetTransaction(UInt256 hash);
        #endregion
    }
}