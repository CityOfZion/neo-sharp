using System;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Persistence
{
    public class NeoSharpRepository : IRepository
    {
        #region Private Fields 

        private readonly IBinarySerializer _serializer;
        private readonly PersistenceConfig _persistenceConfig;
        private readonly IRocksDbRepository _rocksDbRepository;
        private readonly IRedisDbRepository _redisDbRepository;

        #endregion

        #region Constructor 

        public NeoSharpRepository
            (
            IBinarySerializer serializer,
            PersistenceConfig persistenceConfig,
            IRocksDbRepository rocksDbRepository,
            IRedisDbRepository redisDbRepository
            )
        {
            this._serializer = serializer;
            this._persistenceConfig = persistenceConfig;
            this._rocksDbRepository = rocksDbRepository;
            this._redisDbRepository = redisDbRepository;
        }

        #endregion

        #region IRepository Implementation 

        public void AddBlockHeader(BlockHeader blockHeader)
        {
            if (_persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RocksDb)
            {
                _rocksDbRepository.AddBlockHeader(blockHeader);
            }

            if (_persistenceConfig.JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                _redisDbRepository.AddBlockHeader(blockHeader);
            }
        }

        public BlockHeader GetBlockHeader(byte[] hash)
        {
            var ret = _rocksDbRepository.GetBlockHeader(hash);

            if (ret != null) ret.UpdateHash(_serializer, ICrypto.Default);

            return ret;
        }

        public byte[] GetBlockHashFromHeight(uint height)
        {
            return _rocksDbRepository.GetBlockHashFromHeight(height);
        }

        public BlockHeader GetBlockHeader(uint height)
        {
            var hash = _rocksDbRepository.GetBlockHashFromHeight(height);

            if (hash == null) return null;

            return _rocksDbRepository.GetBlockHeader(hash);
        }

        public BlockHeader GetBlockHeaderByTimestamp(int timestamp)
        {
            throw new NotImplementedException();
        }

        public long GetTotalBlockHeight()
        {
            throw new NotImplementedException();
        }

        public void AddTransaction(Transaction transaction)
        {
            if (_persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RocksDb)
            {
                _rocksDbRepository.AddTransaction(transaction);
            }

            if (_persistenceConfig.JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                _redisDbRepository.AddTransaction(transaction);
            }
        }

        public Transaction GetTransaction(byte[] hash)
        {
            var ret = _rocksDbRepository.GetTransaction(hash);

            if (ret != null) ret.UpdateHash(_serializer, ICrypto.Default);

            return ret;
        }

        #endregion
    }
}
