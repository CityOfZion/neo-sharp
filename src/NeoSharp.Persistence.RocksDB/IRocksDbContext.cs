using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeoSharp.Persistence.RocksDB
{
    public interface IRocksDbContext : IDisposable
    {
        Task<byte[]> Get(byte[] key);

        Task<IDictionary<byte[], byte[]>> GetMany(IEnumerable<byte[]> keys);

        Task Save(byte[] key, byte[] content);

        Task Delete(byte[] key);
    }
}