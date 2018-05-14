using System.IO;
using Microsoft.Extensions.Configuration;
using NeoSharp.Core.DI;

namespace NeoSharp.Application.DI
{
    public class ConfigurationModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            var configurationRoot = builder.Build();

            containerBuilder.Register<IConfiguration>(configurationRoot);
        }
    }
}