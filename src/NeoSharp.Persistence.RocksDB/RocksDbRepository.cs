using System;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbRepository : IRepository
    {
        public object GetBlock()
        {
            throw new NotImplementedException();
        }

        public void WriteBlock(object block)
        {
            throw new NotImplementedException();
        }
    }
}
