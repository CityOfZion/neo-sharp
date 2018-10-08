using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Types;
using Validator = NeoSharp.Core.Models.Validator;

namespace NeoSharp.Core.Persistence
{
    public interface IRepository
    {
        #region System

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

        /// <summary>
        /// Gets the version of the blockchain DB
        /// </summary>
        /// <returns></returns>
        Task<string> GetVersion();

        /// <summary>
        /// Sets the version of the blockchain DB
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        Task SetVersion(string version);

        #endregion

        #region Blocks & Headers

        /// <summary>
        /// Adds a block header to the repository storage
        /// </summary>
        /// <param name="blockHeader">Block header</param>
        Task AddBlockHeader(BlockHeader blockHeader);

        /// <summary>
        /// Retrieves a block header by hash
        /// </summary>
        /// <param name="hash">Block id / hash</param>
        /// <returns>Block header with specified id</returns>
        Task<BlockHeader> GetBlockHeader(UInt256 hash);

        /// <summary>
        /// Retrieves a hash by height / index
        /// </summary>
        /// <param name="height">The block height / index to retrieve</param>
        /// <returns>Block hash at specified height / index</returns>
        Task<UInt256> GetBlockHashFromHeight(uint height);

        Task<IEnumerable<UInt256>> GetBlockHashesFromHeights(IEnumerable<uint> heights);

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

        #region State

        /// <summary>
        /// Retrieves an account
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<Account> GetAccount(UInt160 hash);

        /// <summary>
        /// Adds the state of the account
        /// </summary>
        /// <param name="acct"></param>
        /// <returns></returns>
        Task AddAccount(Account acct);

        /// <summary>
        /// Deletes an account
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task DeleteAccount(UInt160 hash);

        /// <summary>
        /// Retrieves coin states by its transaction hash
        /// </summary>
        /// <param name="txHash"></param>
        /// <returns></returns>
        Task<CoinState[]> GetCoinStates(UInt256 txHash);

        /// <summary>
        /// Adds a transaction's outputs as coin states
        /// </summary>
        /// <param name="txHash"></param>
        /// <param name="coinStates"></param>
        /// <returns></returns>
        Task AddCoinStates(UInt256 txHash, CoinState[] coinStates);

        /// <summary>
        /// Deletes all coin states associated with the transaction hash
        /// </summary>
        /// <param name="txHash"></param>
        /// <returns></returns>
        Task DeleteCoinStates(UInt256 txHash);

        /// <summary>
        /// Retrieves a validator by its public key
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        Task<Validator> GetValidator(ECPoint publicKey);

        /// <summary>
        /// Adds a validator
        /// </summary>
        /// <param name="validator"></param>
        /// <returns></returns>
        Task AddValidator(Validator validator);

        /// <summary>
        /// Deletes a validator
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        Task DeleteValidator(ECPoint point);

        /// <summary>
        /// Retrieves an assetId by its assetId
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        Task<Asset> GetAsset(UInt256 assetId);

        /// <summary>
        /// Adds an asset indexed by its assetId
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        Task AddAsset(Asset asset);

        /// <summary>
        /// Deletes an asset
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        Task DeleteAsset(UInt256 assetId);

        /// <summary>
        /// Retrieves a smart contract by its hash
        /// </summary>
        /// <param name="contractHash"></param>
        /// <returns></returns>
        Task<Contract> GetContract(UInt160 contractHash);

        /// <summary>
        /// Adds a smart contract
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        Task AddContract(Contract contract);

        /// <summary>
        /// Delete a smart contract by its hash
        /// </summary>
        /// <param name="contractHash"></param>
        /// <returns></returns>
        Task DeleteContract(UInt160 contractHash);

        /// <summary>
        /// Retrieves a StorageValue by its StorageKey
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<StorageValue> GetStorage(StorageKey key);

        /// <summary>
        /// Adds a StorageValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        Task AddStorage(StorageKey key, StorageValue val);

        /// <summary>
        /// Deletes a StorageKey and its associated StorageValue
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task DeleteStorage(StorageKey key);

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