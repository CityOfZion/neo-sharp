using Microsoft.Extensions.Configuration;

namespace NeoSharp.Modules
{
    public interface IConfigManager
    {
        IConfigurationRoot LoadConfig();
    }
}