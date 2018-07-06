using Microsoft.Extensions.Configuration;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbConfig
    {
        public const string Provider = "RedisDb";

        public string ConnectionString { get; set; }

        public int? DatabaseId { get; set; }

        public bool IsBinaryMode { get; set; }

        public RedisDbConfig(IConfiguration configuration)
        {
            configuration
                .GetSection("persistence")
                .Bind(this);
        }
    }
}
