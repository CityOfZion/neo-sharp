using NeoSharp.Application.DI;
using NeoSharp.Core;
using NeoSharp.DI.SimpleInjector;

namespace NeoSharp.Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var containerBuilder = new SimpleInjectorContainerBuilder();

            containerBuilder.RegisterModule<ClientModule>();
            containerBuilder.RegisterModule<ConfigurationModule>();
            containerBuilder.RegisterModule<LoggingModule>();
            containerBuilder.RegisterModule<NetworkModule>();

            var container = containerBuilder.Build();

            var bootstrapper = container.Resolve<IBootstrapper>();

            bootstrapper.Start(args);
        }
    }
}
