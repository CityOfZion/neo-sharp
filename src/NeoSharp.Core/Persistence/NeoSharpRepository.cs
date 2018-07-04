using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Persistence
{
    public class NeoSharpRepository : IRepository
    {
        #region Private Fields 

        private readonly IBinarySerializer _serializer;
        private readonly IRepository[] _writeRepositories;
        private readonly IRepository _readRepositories;

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
            _serializer = serializer;

            var repositories = new List<IRepository>();

            if (persistenceConfig.BinaryStorageProvider == BinaryStorageProvider.RocksDb)
            {
                repositories.Add(rocksDbRepository);
            }

            if (persistenceConfig.JsonStorageProvider == JsonStorageProvider.RedisDb)
            {
                repositories.Add(redisDbRepository);
            }

            _writeRepositories = repositories.ToArray();
            _readRepositories = repositories.FirstOrDefault();
        }

        #endregion

        #region IRepository Implementation 

        public void AddBlockHeader(BlockHeader blockHeader)
        {
            foreach (var repo in _writeRepositories)
            {
                repo.AddBlockHeader(blockHeader);
            }
        }

        public void SetTotalBlockHeight(uint height)
        {
            foreach (var repo in _writeRepositories)
            {
                repo.SetTotalBlockHeight(height);
            }
        }

        public void AddTransaction(Transaction transaction)
        {
            foreach (var repo in _writeRepositories)
            {
                repo.AddTransaction(transaction);
            }
        }

        public BlockHeader GetBlockHeader(byte[] hash)
        {
            var ret = _readRepositories.GetBlockHeader(hash);

            if (ret != null) ret.UpdateHash(_serializer, ICrypto.Default);

            return ret;
        }

        public byte[] GetBlockHashFromHeight(uint height)
        {
            return _readRepositories.GetBlockHashFromHeight(height);
        }

        public BlockHeader GetBlockHeader(uint height)
        {
            var hash = _readRepositories.GetBlockHashFromHeight(height);

            if (hash == null) return null;

            return GetBlockHeader(hash);
        }

        public uint GetTotalBlockHeight()
        {
            return _readRepositories.GetTotalBlockHeight();
        }

        public Transaction GetTransaction(byte[] hash)
        {
            var ret = _readRepositories.GetTransaction(hash);

            if (ret != null) ret.UpdateHash(_serializer, ICrypto.Default);

            return ret;
        }

        #endregion
    }
}
