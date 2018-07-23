using NeoSharp.Application.DI;
using NeoSharp.Core;
using NeoSharp.Core.DI;
using NeoSharp.DI.SimpleInjector;

namespace NeoSharp.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var containerBuilder = new SimpleInjectorContainerBuilder();

            containerBuilder.RegisterModule<CoreModule>();
            containerBuilder.RegisterModule<ConfigurationModule>();
            containerBuilder.RegisterModule<LoggingModule>();
            containerBuilder.RegisterModule<SerializationModule>();
            containerBuilder.RegisterModule<PersistenceModule>();
            containerBuilder.RegisterModule<ClientModule>();
            containerBuilder.RegisterModule<WalletModule>();
            containerBuilder.RegisterModule<VMModule>();

            var container = containerBuilder.Build();

            var bootstrapper = container.Resolve<IBootstrapper>();

            bootstrapper.Start(args);
        }
    }
}