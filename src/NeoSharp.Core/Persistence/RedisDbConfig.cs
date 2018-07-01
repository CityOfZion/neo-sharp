using Microsoft.Extensions.Configuration;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Persistence
{
    public class RedisDbConfig
    {
        public string ConnectionString { get; set; }

        public int? DatabaseId { get; set; }

        public RedisDbConfig(IConfiguration configuration)
        {
            configuration
                .GetSection("persistence")
                .Bind(this);
        }
    }
}
