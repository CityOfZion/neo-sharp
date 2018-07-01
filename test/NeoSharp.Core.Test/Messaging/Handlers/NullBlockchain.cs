using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Caching;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Test.Messaging.Handlers
{
    class NullBlockchain : IBlockchain
    {
        Block _current = new Block();

        public Block CurrentBlock => _current;

        public BlockHeader LastBlockHeader => CurrentBlock;

        public StampedPool<UInt256, Transaction> MemoryPool => new StampedPool<UInt256, Transaction>(PoolMaxBehaviour.DontAllowMore, 0, x => x.Value.Hash, null);

        public bool AddBlock(Block block)
        {
            throw new NotImplementedException();
        }

        public bool ContainsBlock(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public bool ContainsTransaction(UInt256 hash)
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

        public MetaDataCache<T> GetMetaData<T>() where T : class, ISerializable, new()
        {
            throw new NotImplementedException();
        }

        public Block GetBlock(uint height)
        {
            throw new NotImplementedException();
        }

        public Block GetBlock(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public Contract GetContract(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public Asset GetAsset(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Asset> GetAssets()
        {
            yield break;
        }

        public IEnumerable<Contract> GetContracts()
        {
            yield break;
        }

        public IEnumerable<Block> GetBlocks(IReadOnlyCollection<UInt256> blockHashes)
        {
            throw new NotImplementedException();
        }

        public UInt256 GetBlockHash(uint height)
        {
            throw new NotImplementedException();
        }

        public BlockHeader GetBlockHeader(uint height)
        {
            throw new NotImplementedException();
        }

        public BlockHeader GetBlockHeader(UInt256 hash)
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

        public Block GetNextBlock(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public UInt256 GetNextBlockHash(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public long GetSysFeeAmount(uint height)
        {
            throw new NotImplementedException();
        }

        public long GetSysFeeAmount(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public Transaction GetTransaction(UInt256 hash)
        {
            throw new NotImplementedException();
        }

        public Transaction GetTransaction(UInt256 hash, out int height)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Transaction> GetTransactions(IReadOnlyCollection<UInt256> transactionHashes)
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

        public void AddBlockHeaders(IEnumerable<BlockHeader> blockHeaders)
        {
            throw new NotImplementedException();
        }
    }
}