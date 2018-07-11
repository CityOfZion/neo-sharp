using Microsoft.Extensions.Configuration;

namespace NeoSharp.Persistence.RocksDB
{
    public static class ConfigurationExtensions
    {
        public static void Bind(this IConfiguration config, RocksDbConfig rocksDbConfig)
        {
            rocksDbConfig.FilePath = ParseString(config.GetSection("rocksDbProvider"), "filePath");
        }

        private static string ParseString(IConfiguration config, string section)
        {
            return config.GetSection(section)?.Get<string>();
        }
    }
}