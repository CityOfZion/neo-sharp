using Microsoft.Extensions.Configuration;

namespace NeoSharp.Persistence.RocksDB
{
    public class RocksDbConfig
    {
        public const string Provider = "RocksDb";

        public string FilePath { get; set; }

        public RocksDbConfig(IConfiguration configuration)
        {
            configuration
                .GetSection("persistence")
                .Bind(this);
        }
    }
}