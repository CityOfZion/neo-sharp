using System;

namespace NeoSharp.Persistence.RocksDB
{
    public interface IRocksDbContext : IDisposable
    {
        void Save(byte[] key, byte[] content);

        byte[] Get(byte[] key);
    }
}
