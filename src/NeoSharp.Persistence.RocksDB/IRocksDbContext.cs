using System;
using System.Threading.Tasks;

namespace NeoSharp.Persistence.RocksDB
{
    public interface IRocksDbContext : IDisposable
    {
        Task Save(byte[] key, byte[] content);

        Task<byte[]> Get(byte[] key);

        Task Delete(byte[] key);
    }
}