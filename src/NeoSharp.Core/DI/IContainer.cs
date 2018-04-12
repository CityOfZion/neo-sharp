namespace NeoSharp.Core.DI
{
    public interface IContainer
    {
        TEntity Resolve<TEntity>()
            where TEntity : class;
    }
}