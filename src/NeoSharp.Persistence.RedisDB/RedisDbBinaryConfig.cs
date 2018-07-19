using Microsoft.Extensions.Configuration;

namespace NeoSharp.Persistence.RedisDB
{
    public class RedisDbBinaryConfig
    {
        public const string Provider = "RedisDbBinary";

        public string ConnectionString { get; set; }

        public int? DatabaseId { get; set; }

        public RedisDbBinaryConfig(IConfiguration configuration)
        {
            configuration
                .GetSection("persistence")
                .Bind(this);
        }
    }
}
