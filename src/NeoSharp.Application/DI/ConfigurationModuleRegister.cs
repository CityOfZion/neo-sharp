using System.IO;
using Microsoft.Extensions.Configuration;

namespace NeoSharp.Application.DI
{
    public class ConfigurationModuleRegister
    {
        public void Register(ISimpleInjectorWrapper container)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            var configurationRoot = builder.Build();

            container.Register<IConfiguration>(configurationRoot);
        }
    }
}