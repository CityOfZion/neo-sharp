using NeoSharp.Core.Persistence;
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbRepository : IRepository
    {
        private RedisStreamsHelper _redis;

        public RedisDbRepository()
        {
            //Gubantorious 4.21.18 - We need to make sure we persist this connection, we don't want multiple connections within the app
            //Implementing now for testability, we will want to blend this into the repository / DI patterns
            _redis = new RedisStreamsHelper(ConnectionMultiplexer.Connect("localhost"));
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte[]> GetKeys(DataEntryPrefix entry, byte[] startKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<byte[], byte[]>> GetKeyValues(DataEntryPrefix entry, byte[] startKey)
        {
            throw new NotImplementedException();
        }

        public bool RemoveKey(DataEntryPrefix entry, byte[] key)
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }

        public void SetValue(DataEntryPrefix entry, byte[] key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public void StartTransaction()
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(DataEntryPrefix entry, byte[] key, out byte[] value)
        {
            throw new NotImplementedException();
        }
    }
}