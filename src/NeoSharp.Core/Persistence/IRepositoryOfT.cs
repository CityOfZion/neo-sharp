using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace NeoSharp.Core.Persistence
{
    public interface IRepository<TEntity> 
        where TEntity : NeoEntityBase
    {
        void Create(TEntity entity);

        void Delete(TEntity entity);

        void Update(TEntity entity);

        TEntity GetById(byte[] id);

        ReadOnlyCollection<TEntity> GetAll();

        ReadOnlyCollection<TEntity> GetAll(Expression<Func<TEntity, bool>> expression);
    }
}
