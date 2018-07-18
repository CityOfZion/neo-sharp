using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Wallet.Wrappers;

namespace NeoSharp.Core.DI.Modules
{
    public class HelpersModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IAsyncDelayer, AsyncDelayer>();
            containerBuilder.RegisterSingleton<Crypto, BouncyCastleCrypto>();
            containerBuilder.RegisterSingleton<IFileWrapper, FileWrapper>();
            containerBuilder.RegisterSingleton<IJsonConverter, JsonConverterWrapper>();
        }
    }
}