using System.IO;
using Microsoft.Extensions.Configuration;
using SimpleInjector;

namespace NeoSharp.Application.DI
{
    public class ConfigurationPackage
    {
        public static void RegisterServices(Container container)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            var configurationRoot = builder.Build();

            container.RegisterInstance<IConfiguration>(configurationRoot);
        }
    }
}