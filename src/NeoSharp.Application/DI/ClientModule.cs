using System.Linq;
using NeoSharp.Application.Client;
using NeoSharp.Core;
using NeoSharp.Core.DI;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Application.DI
{
    public class ClientModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.Register<IBootstrapper, Bootstrapper>();
            containerBuilder.RegisterSingleton<IPrompt, Prompt>();
            containerBuilder.RegisterSingleton<IPromptUserVariables, PromptUserVariables>();

            containerBuilder.RegisterSingleton<IConsoleHandler, ConsoleHandler>();

            // Get prompt controllers

            var promptHandlerTypes = typeof(IPromptController).Assembly
               .GetExportedTypes()
               .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract && typeof(IPromptController).IsAssignableFrom(t))
               .ToArray();

            containerBuilder.RegisterCollection(typeof(IPromptController), promptHandlerTypes);
        }
    }
}