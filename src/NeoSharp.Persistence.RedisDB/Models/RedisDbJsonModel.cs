using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbJsonModel : IDbModel
    {
        private readonly IDbJsonContext _dbContext;

        public RedisDbJsonModel(IDbJsonContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task Create<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash, TEntity entity) where TEntity : Entity
        {
            var key = BuildKey(dataEntryPrefix, hash);
            var buffer = JsonConvert.SerializeObject(entity);

            return _dbContext.Create(key, buffer);
        }

        public Task Delete(DataEntryPrefix dataEntryPrefix, UInt256 hash)
        {
            var key = BuildKey(dataEntryPrefix, hash);
            return _dbContext.Delete(key);
        }

        public async Task<TEntity> Get<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash) where TEntity : Entity
        {
            var key = BuildKey(dataEntryPrefix, hash);
            var buffer = await _dbContext.Get(key);

            return JsonConvert.DeserializeObject<TEntity>(buffer);
        }

        public Task Update<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash, TEntity entity) where TEntity : Entity
        {
            //Redis allows overwrites of key values without delete
            return Create(dataEntryPrefix, hash, entity);
        }

        private string BuildKey(DataEntryPrefix dataEntryPrefix, UInt256 hash)
        {
            return string.Concat(dataEntryPrefix.ToString(),hash.ToString());
        }
    }
}
