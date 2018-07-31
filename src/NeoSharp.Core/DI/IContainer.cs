using System;

namespace NeoSharp.Core.DI
{
    public interface IContainer
    {
        object Resolve(Type serviceType);

        TEntity Resolve<TEntity>() where TEntity : class;

        bool TryResolve(Type parameterType, out object obj);
    }
}