using Microsoft.Extensions.Configuration;

namespace NeoSharp.Persistence.RedisDB
{
    public static class ConfigurationExtensions
    {
        public static void Bind(this IConfiguration config, RedisDbBinaryConfig redisDbBinaryConfig)
        {
            var redisDbProviderConfig = config.GetSection("redisDbBinaryProvider");

            redisDbBinaryConfig.ConnectionString = ParseString(redisDbProviderConfig, "connectionString");
            redisDbBinaryConfig.DatabaseId = ParseUInt16(redisDbProviderConfig, "databaseId");
        }
        
        public static void Bind(this IConfiguration config, RedisDbJsonConfig redisDbBinaryConfig)
        {
            var redisDbProviderConfig = config.GetSection("redisDbJsonProvider");

            redisDbBinaryConfig.ConnectionString = ParseString(redisDbProviderConfig, "connectionString");
            redisDbBinaryConfig.DatabaseId = ParseUInt16(redisDbProviderConfig, "databaseId");
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