using System.Threading.Tasks;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence
{
    public interface IDbModel
    {
        Task Create<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash, TEntity entity)
            where TEntity : Entity;

        Task Update<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash, TEntity entity)
            where TEntity : Entity;

        Task DeleteByHash(DataEntryPrefix dataEntryPrefix, UInt256 hash);

        Task<TEntity> GetByHash<TEntity>(DataEntryPrefix dataEntryPrefix, UInt256 hash)
            where TEntity : Entity;
    }
}
