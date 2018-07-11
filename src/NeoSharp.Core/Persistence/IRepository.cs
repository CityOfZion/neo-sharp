using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence
{
    public interface IRepository
    {
        #region Blocks & Headers
        
        /// <summary>
        /// Adds a block header to the repository storage
        /// </summary>
        /// <param name="blockHeader">Block header</param>
        Task AddBlockHeader(BlockHeaderBase blockHeader);

        /// <summary>
        /// Retrieves a block header by hash
        /// </summary>
        /// <param name="hash">Block id / hash</param>
        /// <returns>Block header with specified id</returns>
        Task<BlockHeader> GetBlockHeaderExtended(UInt256 hash);

        /// <summary>
        /// Retrieves a block header by hash
        /// </summary>
        /// <param name="hash">Block id / hash</param>
        /// <returns>Block header with specified id</returns>
        Task<BlockHeaderBase> GetBlockHeader(UInt256 hash);

        /// <summary>
        /// Retrieves a hash by height / index
        /// </summary>
        /// <param name="height">The block height / index to retrieve</param>
        /// <returns>Block hash at specified height / index</returns>
        Task<UInt256> GetBlockHashFromHeight(uint height);

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

        /// <summary>
        /// Retrieves the total / current block header height
        /// </summary>
        /// <returns>Total / current block header height</returns>
        Task<uint> GetTotalBlockHeaderHeight();

        /// <summary>
        /// Set the total/ current block header height
        /// </summary>
        /// <param name="height">Total / current block header height</param>
        Task SetTotalBlockHeaderHeight(uint height);

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

        #region Index

        /// <summary>
        /// Retrieves the height of the index
        /// </summary>
        /// <returns>Height of the index</returns>
        Task<uint> GetIndexHeight();
        /// <summary>
        /// Sets the height of the index
        /// </summary>
        /// <param name="height">New height of the index</param>
        Task SetIndexHeight(uint height);
        /// <summary>
        /// Gets all confirmed CoinReferences of an address. These are spendable coins.
        /// </summary>
        /// <param name="scriptHash">Address</param>
        Task<HashSet<CoinReference>> GetIndexConfirmed(UInt160 scriptHash);
        /// <summary>
        /// Sets the confirmed CoinReferences of an address. These are spendable coins.
        /// </summary>
        /// <param name="scriptHash">Address</param>
        /// <param name="coinReferences">List of CoinReferences to write</param>
        Task SetIndexConfirmed(UInt160 scriptHash, HashSet<CoinReference> coinReferences);
        /// <summary>
        /// Gets all claimable CoinReferences of an address. These are spent coins with unclaimed GAS.
        /// </summary>
        /// <param name="scriptHash">Address</param>
        Task<HashSet<CoinReference>> GetIndexClaimable(UInt160 scriptHash);
        /// <summary>
        /// Sets the claimable CoinReferences of an address. These are spent coins with unclaimed GAS.
        /// </summary>
        /// <param name="scriptHash">Address</param>
        /// <param name="coinReferences">List of CoinReferences to write</param>
        Task SetIndexClaimable(UInt160 scriptHash, HashSet<CoinReference> coinReferences);
  
        #endregion
    }
}