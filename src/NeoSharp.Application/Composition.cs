using NeoSharp.Application.DI;
using SimpleInjector;

namespace NeoSharp.Application
{
    public static class Composition
    {
        public static Container Compose()
        {
            // Create container
            Container container = new Container();

            ConfigurationPackage.RegisterServices(container);
            LoggingPackage.RegisterServices(container);
            ClientPackage.RegisterServices(container);            
            NetworkPackage.RegisterServices(container);

            // Verify
            container.Verify();

            return container;
        }
    }
}