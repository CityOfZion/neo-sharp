using Microsoft.Extensions.Configuration;

namespace NeoSharp.Core.Persistence
{
    public static class ConfigurationExtensions
    {
        public static void Bind(this IConfiguration config, PersistenceConfig persistenceConfig)
        {
            persistenceConfig.Provider = config.GetSection("provider")?.Get<string>();
        }
   }
}