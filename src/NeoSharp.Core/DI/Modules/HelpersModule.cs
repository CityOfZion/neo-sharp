using NeoSharp.Core.Helpers;

namespace NeoSharp.Core.DI.Modules
{
    public class HelpersModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IAsyncDelayer, AsyncDelayer>();
        }
    }
}
