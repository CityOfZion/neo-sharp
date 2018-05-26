using NeoSharp.Core.DI.Modules;

namespace NeoSharp.Core.DI
{
    public class CoreModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterModule<BlockchainModule>();
            containerBuilder.RegisterModule<NetworkModule>();
            containerBuilder.RegisterModule<HelpersModule>();
        }
    }
}