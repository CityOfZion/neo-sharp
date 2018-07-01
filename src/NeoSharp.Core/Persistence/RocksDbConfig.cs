using Microsoft.Extensions.Configuration;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Persistence
{
    public class RocksDbConfig
    {
        public string FilePath { get; set; }

        public RocksDbConfig(IConfiguration configuration)
        {
            configuration
                .GetSection("persistence")
                .Bind(this);
        }
    }
}
