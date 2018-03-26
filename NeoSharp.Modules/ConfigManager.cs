using Microsoft.Extensions.Configuration;
using System.IO;
using NeoSharp.Modules;

namespace NeoSharp.Modules
{
    public class ConfigManager
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
