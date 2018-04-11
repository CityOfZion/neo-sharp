using NeoSharp.Application.Client;
using NeoSharp.Core;

namespace NeoSharp.Application.DI
{
    public class ClientModuleRegister 
    {
        public void Register(ISimpleInjectorWrapper container)
        {
            container.Register<IBootstrapper, ClientManager>();

            container.RegisterSingleton<IPrompt, Prompt>();
            container.RegisterSingleton<IConsoleReader, ConsoleReader>();
            container.RegisterSingleton<IConsoleWriter, ConsoleWriter>();
        }
    }

}
