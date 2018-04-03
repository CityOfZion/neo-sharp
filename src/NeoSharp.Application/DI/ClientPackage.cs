using NeoSharp.Application.Client;
using SimpleInjector;

namespace NeoSharp.Application.DI
{
    public static class ClientPackage
    {
        public static void RegisterServices(Container container)
        {
            container.Register<IClientManager, ClientManager>(Lifestyle.Singleton);
            container.Register<IPrompt, Prompt>(Lifestyle.Singleton);
            container.Register<IConsoleReader, ConsoleReader>(Lifestyle.Singleton);
            container.Register<IConsoleWriter, ConsoleWriter>(Lifestyle.Singleton);
        }     
    }

}
