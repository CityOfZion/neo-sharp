using System.Threading.Tasks;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Persistence
{
    public interface IDbModel
    {
        Task Create<TEntity>(TEntity entity, DataEntryPrefix dataEntryPrefix)
            where TEntity : NeoEntityBase;

        Task Delete<TEntity>(TEntity entity)
            where TEntity : NeoEntityBase;

        Task Update<TEntity>(TEntity entity)
            where TEntity : NeoEntityBase;

        Task<TEntity> GetByHash<TEntity>(UInt256 hash, DataEntryPrefix dataEntryPrefix)
            where TEntity : NeoEntityBase;
    }
}
