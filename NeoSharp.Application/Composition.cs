using NeoSharp.Client.DI;
using NeoSharp.Core.DI;
using NeoSharp.Core.Network.DI;
using SimpleInjector;

namespace NeoSharp.Application
{
    public static class Composition
    {
        public static Container Compose()
        {
            // Create container
            Container container = new Container();

            // Register services
            CorePackage.RegisterServices(container);            
            ClientPackage.RegisterServices(container);            
            NetworkPackage.RegisterServices(container);

            // Verify
            container.Verify();

            return container;
        }
    }
}