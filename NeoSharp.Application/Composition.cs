using NeoSharp.Client.DI;
using NeoSharp.Logging.DI;
using NeoSharp.Network.DI;
using SimpleInjector;

namespace NeoSharp.Application
{
    public static class Composition
    {
        public static void Compose(out Container container)
        {
            container = new Container();

            ClientPackage.RegisterServices(container);
            LoggingPackage.RegisterServices(container);
            NetworkPackage.RegisterServices(container);

            // verify
            container.Verify();
        }
    }
}
