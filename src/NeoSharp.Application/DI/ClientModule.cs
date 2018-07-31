using System;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.Application.Client;
using NeoSharp.Application.Controllers;
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
            containerBuilder.RegisterSingleton<IConsoleReader, ConsoleReader>();
            containerBuilder.RegisterSingleton<IConsoleWriter, ConsoleWriter>();

            // Get prompt controllers

            var promptHandlerTypes = typeof(IPromptController).Assembly
               .GetExportedTypes()
               .Where(t => t.IsClass && !t.IsAbstract && typeof(IPromptController).IsAssignableFrom(t))
               .ToArray();

            containerBuilder.RegisterSingleton(() => new PromptControllerFactory(promptHandlerTypes));
        }
    }
}