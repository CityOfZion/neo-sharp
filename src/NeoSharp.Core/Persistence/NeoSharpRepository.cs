using System;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Persistence
{
    public class NeoSharpRepository : IRepository
    {
        #region Private Fields 
        private readonly PersistenceConfig _persistenceConfig;
        private readonly IRocksDbRepository _rocksDbRepository;
        private readonly IRedisDbRepository _redisDbRepository;

        #endregion

        #region Constructor 
        public NeoSharpRepository(
            PersistenceConfig persistenceConfig, 
            IRocksDbRepository rocksDbRepository, 
            IRedisDbRepository redisDbRepository)
        {
            this._persistenceConfig = persistenceConfig;
            this._rocksDbRepository = rocksDbRepository;
            this._redisDbRepository = redisDbRepository;
        }
        #endregion

        #region IRepository Implementation 
        public void AddBlockHeader(BlockHeader blockHeader)
        {
            if (this._persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RocksDb)
            {
                this._rocksDbRepository.AddBlockHeader(blockHeader);
            }

            if (this._persistenceConfig.JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                this._redisDbRepository.AddBlockHeader(blockHeader);
            }
        }

        public BlockHeader GetBlockHeaderById(byte[] id)
        {
            throw new NotImplementedException();
        }

        public BlockHeader GetBlockHeaderById(string id)
        {
            throw new NotImplementedException();
        }

        public BlockHeader GetBlockHeaderByHeight(int height)
        {
            throw new NotImplementedException();
        }

        public BlockHeader GetBlockHeaderByTimestamp(int timestamp)
        {
            throw new NotImplementedException();
        }

        public object GetRawBlock(string id)
        {
            throw new NotImplementedException();
        }

        public object GetRawBlock(byte[] id)
        {
            throw new NotImplementedException();
        }

        public long GetTotalBlockHeight()
        {
            throw new NotImplementedException();
        }

        public void AddTransaction(Transaction transaction)
        {
            throw new NotImplementedException();
        }

        public Transaction GetTransaction(byte[] id)
        {
            throw new NotImplementedException();
        }

        public Transaction GetTransaction(string id)
        {
            throw new NotImplementedException();
        }

        public Transaction[] GetTransactionsForBlock(byte[] id)
        {
            throw new NotImplementedException();
        }

        public Transaction[] GetTransactionsForBlock(string id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
