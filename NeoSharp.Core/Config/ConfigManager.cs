using Microsoft.Extensions.Configuration;
using System.IO;

namespace NeoSharp.Core.Config
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
