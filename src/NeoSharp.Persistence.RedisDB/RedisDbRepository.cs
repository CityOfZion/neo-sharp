using NeoSharp.Core.Persistence;
using System;
using System.Collections.Generic;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbRepository : IRepository
    {
        public ISnapshot CreateSnapshot()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<byte[]> GetKeys(DataEntry entry, byte[] startKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<KeyValuePair<byte[], byte[]>> GetKeyValues(DataEntry entry, byte[] startKey)
        {
            throw new NotImplementedException();
        }

        public void Prepare(DataEntry entry)
        {
            throw new NotImplementedException();
        }

        public bool RemoveKey(DataEntry entry, byte[] key)
        {
            throw new NotImplementedException();
        }

        public void SetValue(DataEntry entry, byte[] key, byte[] value)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(DataEntry entry, byte[] key, out byte[] value)
        {
            throw new NotImplementedException();
        }
    }
}