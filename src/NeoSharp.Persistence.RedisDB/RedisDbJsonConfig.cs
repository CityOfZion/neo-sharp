using Microsoft.Extensions.Configuration;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbJsonConfig
    {
        public const string Provider = "RedisDbJson";

        public string ConnectionString { get; set; }

        public int? DatabaseId { get; set; }

        public RedisDbJsonConfig(IConfiguration configuration)
        {
            configuration
                .GetSection("persistence")
                .Bind(this);
        }
    }
}
