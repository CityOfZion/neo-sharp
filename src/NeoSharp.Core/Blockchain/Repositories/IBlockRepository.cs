using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Repositories
{
    public interface IBlockRepository
    {
        /// <summary>
        /// Set the total/ current block height
        /// </summary>
        /// <param name="height">Total / current block height</param>
        Task SetTotalBlockHeight(uint height);

        /// <summary>
        /// Retrieves the total / current block height
        /// </summary>
        /// <returns>Total / current block height</returns>
        Task<uint> GetTotalBlockHeight();

        /// <summary>
        /// Retrieves the total / current block header height
        /// </summary>
        /// <returns>Total / current block header height</returns>
        Task<uint> GetTotalBlockHeaderHeight();

        /// <summary>
        /// Return the corresponding block information according to the specified height
        /// </summary>
        /// <param name="height">The height of the blockchain.</param>
        /// <returns>Block</returns>
        Task<Block> GetBlock(uint height);

        /// <summary>
        /// Return the corresponding block information according to the specified height
        /// </summary>
        /// <param name="hash"></param>
        /// <returns>Block</returns>
        Task<Block> GetBlock(UInt256 hash);

        /// <summary>
        /// Get blocks
        /// </summary>
        /// <param name="blockHashes">Block hashes</param>
        /// <returns>List of Blocks</returns>
        Task<IEnumerable<Block>> GetBlocks(IReadOnlyCollection<UInt256> blockHashes);

        /// <summary>
        /// Returns the hash of the corresponding block based on the specified height
        /// </summary>
        /// <param name="height">The height of the blockchain.</param>
        /// <returns></returns>
        Task<UInt256> GetBlockHash(uint height);

        Task<IEnumerable<UInt256>> GetBlockHashes(uint fromHeight, uint toHeight);

        /// <summary>
        /// Returns the information for the next block based on the specified hash value
        /// </summary>
        /// <param name="hash">Block hash.</param>
        /// <returns>Block</returns>
        Task<Block> GetNextBlock(UInt256 hash);

        /// <summary>
        /// Returns the hash value of the next block based on the specified hash value
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<UInt256> GetNextBlockHash(UInt256 hash);

        /// <summary>
        /// Returns the total amount of system costs contained in the corresponding block and all previous blocks based on the specified block height
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        Task<long> GetSysFeeAmount(uint height);

        /// <summary>
        /// Returns the total amount of system charges contained in the corresponding block and all previous blocks based on the specified block hash value
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        long GetSysFeeAmount(UInt256 hash);

        /// <summary>
        /// Return the corresponding block header information according to the specified height
        /// </summary>
        /// <param name="height"></param>
        /// <returns></returns>
        Task<BlockHeader> GetBlockHeader(uint height);

        /// <summary>
        /// Returns the corresponding block header information according to the specified hash value
        /// </summary>
        /// <param name="hash"></param>
        /// <returns></returns>
        Task<BlockHeader> GetBlockHeader(UInt256 hash);

        /// <summary>
        /// Add block header to the repository and update the block header height.
        /// This two operations are always together and make sense on this level to put them together.
        /// </summary>
        /// <param name="blockHeader"></param>
        /// <returns></returns>
        Task AddBlockHeader(BlockHeader blockHeader);

        /// <summary>
        /// Update block header in the repository
        /// </summary>
        /// <param name="blockHeader"></param>
        /// <returns></returns>
        Task UpdateBlockHeader(BlockHeader blockHeader);
    }
}
