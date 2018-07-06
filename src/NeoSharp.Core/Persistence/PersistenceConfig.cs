using System.IO;
using Microsoft.Extensions.Configuration;

namespace NeoSharp.Core.Persistence
{
    public class PersistenceConfig
    {
        private static PersistenceConfig _persistenceConfig;

        public string Provider { get; internal set; }

        public static PersistenceConfig Instance()
        {
            return _persistenceConfig ?? (_persistenceConfig = new PersistenceConfig());
        }

        public PersistenceConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);

            var configuration = (IConfiguration)builder.Build();

            configuration?
                .GetSection("persistence")?
                .Bind(this);
        }
    }
}
