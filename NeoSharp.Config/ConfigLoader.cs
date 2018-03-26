using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace NeoSharp.Config
{
    public class ConfigLoader : IConfigLoader
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
