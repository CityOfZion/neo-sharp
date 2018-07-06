using Microsoft.Extensions.Configuration;

namespace NeoSharp.Persistence.RedisDB
{
    public static class ConfigurationExtensions
    {
        public static void Bind(this IConfiguration config, RedisDbConfig redisDbConfig)
        {
            var redisDbProviderConfig = config.GetSection("redisDbProvider");

            redisDbConfig.ConnectionString = ParseString(redisDbProviderConfig, "connectionString");
            redisDbConfig.DatabaseId = ParseUInt16(redisDbProviderConfig, "databaseId");
            redisDbConfig.IsBinaryMode = ParseBool(redisDbProviderConfig, "isBinaryMode");
        }

        private static ushort ParseUInt16(IConfiguration config, string section, ushort defaultValue = 0)
        {
            var value = config.GetSection(section)?.Get<ushort>();
            return value ?? defaultValue;
        }

        private static bool ParseBool(IConfiguration config, string section, bool defaultValue = true)
        {
            var value = config.GetSection(section)?.Get<bool>();
            return value ?? defaultValue;
        }

        private static string ParseString(IConfiguration config, string section)
        {
            return config.GetSection(section)?.Get<string>();
        }
    }
}