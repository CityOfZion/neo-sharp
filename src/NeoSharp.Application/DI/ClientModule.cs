using NeoSharp.Application.Client;
using NeoSharp.BinarySerialization.DI;
using NeoSharp.Core;
using NeoSharp.Core.DI;

namespace NeoSharp.Application.DI
{
    public class ClientModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.Register<IBootstrapper, Bootstrapper>();

            containerBuilder.RegisterSingleton<IPrompt, Prompt>();
            containerBuilder.RegisterSingleton<IConsoleReader, ConsoleReader>();
            containerBuilder.RegisterSingleton<IConsoleWriter, ConsoleWriter>();
            containerBuilder.RegisterSingleton<ICryptoInitializer, CryptoInitializer>();
            containerBuilder.RegisterSingleton<IBinaryInitializer, BinaryInitializer>();
        }
    }

}
