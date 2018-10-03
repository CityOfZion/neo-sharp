using NeoSharp.Core.Helpers;
using NeoSharp.Core.Wallet.Wrappers;
using NeoSharp.Cryptography;

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

            containerBuilder.OnBuild += c =>
            {
                InitializeCrypto(c.Resolve<Crypto>());
            };
        }

        private static void InitializeCrypto(Crypto crypto)
        {
            Crypto.Initialize(crypto);
        }
    }
}