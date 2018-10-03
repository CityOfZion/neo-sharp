using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain
{
    /// <summary>
    ///     An index of relevant CoinReferences on the blockchain.
    ///     Spendable references are coins that can be used as TransactionInputs in constructing transactions.
    ///     Claimable references are coins that can be used as claims in constructing ClaimTransactions.
    /// </summary>
    public class CoinIndex : ICoinIndex
    {
        private readonly IRepository _repository;

        public CoinIndex(IRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        ///     Retrieves spendable coins of an address
        /// </summary>
        /// <param name="hash">The hash of the address</param>
        /// <returns></returns>
        public async Task<HashSet<CoinReference>> GetSpendable(UInt160 hash)
        {
            return await _repository.GetIndexConfirmed(hash);
        }

        /// <summary>
        ///     Retrieves claimable coins of an address
        /// </summary>
        /// <param name="hash">The hash of the address</param>
        /// <returns></returns>
        public async Task<HashSet<CoinReference>> GetClaimable(UInt160 hash)
        {
            return await _repository.GetIndexClaimable(hash);
        }

        /// <summary>
        ///     Retrieves the height of this index
        /// </summary>
        /// <returns></returns>
        public async Task<uint> GetHeight()
        {
            return await _repository.GetIndexHeight();
        }

        /// <summary>
        ///     Processes the next block into the index
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public async Task IndexBlock(Block block)
        {
            var currentHeight = await GetHeight();
            if (block.Index != currentHeight + 1)
                throw new ArgumentException(
                    $"Unable to index block: Requires next block to be {currentHeight + 1} but received block {block.Index}");

            // TODO #387: Index Spendable
            // TODO #388: Index Claimable
            await _repository.SetIndexHeight(currentHeight + 1);
        }
    }
}