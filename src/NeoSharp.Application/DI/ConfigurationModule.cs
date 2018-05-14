using Microsoft.Extensions.Configuration;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.Interfaces;
using NeoSharp.Core.DI;
using System.IO;

namespace NeoSharp.Application.DI
{
    public class ConfigurationModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true);
            var configurationRoot = builder.Build();

            containerBuilder.Register<IConfiguration>(configurationRoot);
            containerBuilder.RegisterSingleton<IBinarySerializer, BinarySerializer>();
            containerBuilder.RegisterSingleton<IBinaryDeserializer, BinaryDeserializer>();
        }
    }
}