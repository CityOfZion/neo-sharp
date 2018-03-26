using NeoSharp.Client.DI;
using NeoSharp.Logging.DI;
using NeoSharp.Network.DI;
using NeoSharp.Config.DI;
using SimpleInjector;

namespace NeoSharp.Application
{
    public static class Composition
    {
        public static void Compose(out Container container)
        {
            container = new Container();

            LoggingPackage.RegisterServices(container);
            ConfigPackage.RegisterServices(container);
            ClientPackage.RegisterServices(container);            
            NetworkPackage.RegisterServices(container);

            // verify
            container.Verify();
        }
    }
}
