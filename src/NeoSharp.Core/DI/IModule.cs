namespace NeoSharp.Core.DI
{
    public interface IModule
    {
        void Register(IContainerBuilder containerBuilder);
    }
}