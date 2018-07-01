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
        /// Retrieves a block header by identifier
        /// </summary>
        /// <param name="id">Block id / hash</param>
        /// <returns>Block header with specified id</returns>
        BlockHeader GetBlockHeaderById(byte[] id);

        /// <summary>
        /// Retrieves a block header by identifier
        /// </summary>
        /// <param name="id">Block id / hash</param>
        /// <returns>Block header with specified id</returns>
        BlockHeader GetBlockHeaderById(string id);

        /// <summary>
        /// Retrieves a block header by height / index
        /// </summary>
        /// <param name="height">The block height / index to retrieve</param>
        /// <returns>Block header at specified height / index</returns>
        BlockHeader GetBlockHeaderByHeight(int height);

        /// <summary>
        /// Retrieves a block header by timestamp
        /// </summary>
        /// <param name="timestamp">The block timestamp to retrieve the block at</param>
        /// <returns>Block header at the specified timestamp</returns>
        BlockHeader GetBlockHeaderByTimestamp(int timestamp);

        /// <summary>
        /// Retrieves the raw bytes for a block by identifier
        /// </summary>
        /// <param name="id">Block id / hash</param>
        /// <returns>Raw bytes for the block</returns>
        object GetRawBlock(string id);

        /// <summary>
        /// Retrieves the raw bytes for a block by identifier
        /// </summary>
        /// <param name="id">Block id / hash</param>
        /// <returns>Raw bytes for the block</returns>
        object GetRawBlock(byte[] id);

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
        /// <param name="id">Identifier / hash of the transaction</param>
        /// <returns>Transaction with the specified id / hash</returns>
        Transaction GetTransaction(byte[] id);

        /// <summary>
        /// Retrieves a transaction by identifier / hash
        /// </summary>
        /// <param name="id">Identifier / hash of the transaction</param>
        /// <returns>Transaction with the specified id / hash</returns>
        Transaction GetTransaction(string id);

        /// <summary>
        /// Retrieves transactions for the specified block
        /// </summary>
        /// <param name="id">Identifier / hash of the block to retrieve transactions for</param>
        /// <returns>Transactions for the specified block</returns>
        Transaction[] GetTransactionsForBlock(byte[] id);

        /// <summary>
        /// Retrieves transactions for the specified block
        /// </summary>
        /// <param name="id">Identifier / hash of the block to retrieve transactions for</param>
        /// <returns>Transactions for the specified block</returns>
        Transaction[] GetTransactionsForBlock(string id);
        #endregion
    }
}