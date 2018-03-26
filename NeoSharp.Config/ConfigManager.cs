using Microsoft.Extensions.Configuration;
using System.IO;
using NeoSharp.Modules;

namespace NeoSharp.Config
{
    public class ConfigManager : IConfigManager
    {
        public IConfigurationRoot LoadConfig()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            return builder.Build();
        }
    }
}
