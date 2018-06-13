using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;

namespace NeoSharp.Persistence.RocksDB
{
    public class BlockHeaderRepository : RocksDbRepository<BlockHeader>, IBlockHeaderRepository
    {
        public BlockHeaderRepository(IRepositoryConfiguration config, IBinarySerializer serializer, IBinaryDeserializer deserializer) 
            : base(config, serializer, deserializer)
        {
        }
    }
}
