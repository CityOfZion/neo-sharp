using NeoSharp.Core.Models;
using System.Collections.Generic;

namespace NeoSharp.Core.Persistence
{
    public interface IRepository
    {
        #region Repository Configuration
        /// <summary>
        /// Initializes the repository connection
        /// </summary>
        /// <param name="connection">Connection string</param>
        /// <param name="database">Database specifier</param>
        void Initialize(string connection, string database);
        #endregion


        #region Blocks
        /// <summary>
        /// Adds a block to the repository storage
        /// </summary>
        /// <param name="block">Block to be added</param>
        void AddBlock(Block block);

        /// <summary>
        /// Retrieves a block by identifier
        /// </summary>
        /// <param name="id">Block id / hash</param>
        /// <returns>Block with specified id</returns>
        Block GetBlockById(byte[] id);

        /// <summary>
        /// Retrieves a block by identifier
        /// </summary>
        /// <param name="id">Block id / hash</param>
        /// <returns>Block with specified id</returns>
        Block GetBlockById(string id);

        /// <summary>
        /// Retrieves a block by height / index
        /// </summary>
        /// <param name="height">The block height / index to retrieve</param>
        /// <returns>Block at specified height / index</returns>
        Block GetBlockByHeight(int height);

        /// <summary>
        /// Retrieves a block by timestamp
        /// </summary>
        /// <param name="timestamp">The block timestamp to retrieve the block at</param>
        /// <returns>Block at the specified timestamp</returns>
        Block GetBlockByTimestamp(int timestamp);

        /// <summary>
        /// Retrieves the raw bytes for a block by identifier
        /// </summary>
        /// <param name="id">Block id / hash</param>
        /// <returns>Raw bytes for the block</returns>
        byte[] GetRawBlockBytes(string id);

        /// <summary>
        /// Retrieves the raw bytes for a block by identifier
        /// </summary>
        /// <param name="id">Block id / hash</param>
        /// <returns>Raw bytes for the block</returns>
        byte[] GetRawBlockBytes(byte[] id);

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