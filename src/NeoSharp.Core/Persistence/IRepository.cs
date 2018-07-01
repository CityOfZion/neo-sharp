using NeoSharp.Core.Models;

namespace NeoSharp.Core.Persistence
{
    public interface IRepository
    {
        #region Blocks

        /// <summary>
        /// Adds a block header to the repository storage
        /// </summary>
        /// <param name="blockHeader"></param>
        void AddBlockHeader(BlockHeader blockHeader);

        /// <summary>
        /// Retrieves a hash by height / index
        /// </summary>
        /// <param name="height">The block height / index to retrieve</param>
        /// <returns>Block hash at specified height / index</returns>
        byte[] GetBlockHashFromHeight(uint height);

        /// <summary>
        /// Retrieves a block header by hash
        /// </summary>
        /// <param name="hash">Block id / hash</param>
        /// <returns>Block header with specified id</returns>
        BlockHeader GetBlockHeader(byte[] hash);

        /// <summary>
        /// Retrieves the total / current block height
        /// </summary>
        /// <returns>Total / current block height</returns>
        long GetTotalBlockHeight();

        #endregion

        #region Transactions

        /// <summary>
        /// Adds a transaction to the repository
        /// </summary>
        /// <param name="transaction">Transaction to add</param>
        void AddTransaction(Transaction transaction);

        /// <summary>
        /// Retrieves a transaction by identifier / hash
        /// </summary>
        /// <param name="hash">Identifier / hash of the transaction</param>
        /// <returns>Transaction with the specified id / hash</returns>
        Transaction GetTransaction(byte[] hash);

        #endregion
    }
}