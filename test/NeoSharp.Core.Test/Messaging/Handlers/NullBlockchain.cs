using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Messaging.Handlers
{
    internal class NullBlockchain : IBlockchain
    {
        public Block CurrentBlock { get; } = new Block();

        public BlockHeader LastBlockHeader => CurrentBlock.GetBlockHeader();

        public StampedPool<UInt256, Transaction> MemoryPool => new StampedPool<UInt256, Transaction>(PoolMaxBehaviour.DontAllowMore, 0, x => x.Value.Hash, null);

        public Task InitializeBlockchain()
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ContainsTransaction(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public bool ContainsUnspent(CoinReference input)
        {
            throw new NotImplementedException();
        }

        public bool ContainsUnspent(UInt256 hash, ushort index)
        {
            throw new NotImplementedException();
        }

        public Task<Block> GetBlock(uint height)
        {
            throw new NotImplementedException();
        }

        public Task<Block> GetBlock(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public Task<Contract> GetContract(UInt160 hash)
        {
            throw new NotImplementedException();
        }

        public Task<Asset> GetAsset(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Asset>> GetAssets()
        {
            throw new NotFiniteNumberException();
        }

        public Task<IEnumerable<Contract>> GetContracts()
        {
            throw new NotFiniteNumberException();
        }

        public Task<IEnumerable<Block>> GetBlocks(IReadOnlyCollection<UInt256> blockHashes)
        {
            throw new NotImplementedException();
        }

        public Task<UInt256> GetBlockHash(uint height)
        {
            throw new NotImplementedException();
        }

        public Task<BlockHeader> GetBlockHeader(uint height)
        {
            throw new NotImplementedException();
        }

        public Task<BlockHeader> GetBlockHeader(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public ECPoint[] GetValidators()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ECPoint> GetValidators(IEnumerable<Transaction> others)
        {
            throw new NotImplementedException();
        }

        public Task<Block> GetNextBlock(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public Task<UInt256> GetNextBlockHash(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public Task<long> GetSysFeeAmount(uint height)
        {
            throw new NotImplementedException();
        }

        public long GetSysFeeAmount(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public Task<Transaction> GetTransaction(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public Transaction GetTransaction(UInt256 hash, out int height)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Transaction>> GetTransactions(IReadOnlyCollection<UInt256> transactionHashes)
        {
            throw new NotImplementedException();
        }

        public TransactionOutput GetUnspent(UInt256 hash, ushort index)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TransactionOutput> GetUnspent(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public bool IsDoubleSpend(Transaction tx)
        {
            throw new NotImplementedException();
        }

        public Task AddBlockHeaders(IEnumerable<BlockHeader> blockHeaders)
        {
            throw new NotImplementedException();
        }
    }
}