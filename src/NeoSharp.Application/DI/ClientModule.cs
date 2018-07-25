using NeoSharp.Application.Client;
using NeoSharp.BinarySerialization;
using NeoSharp.Core;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.DI;

namespace NeoSharp.Application.DI
{
    public class ClientModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.Register<IBootstrapper, Bootstrapper>();

            containerBuilder.RegisterSingleton<IPrompt, Prompt>();
            containerBuilder.RegisterSingleton<IConsoleReader, ConsoleReader>();
            containerBuilder.RegisterSingleton<IConsoleWriter, ConsoleWriter>();

            containerBuilder.OnBuild += c =>
            {
                InitializeCrypto(c.Resolve<Crypto>());
                InitializeBinarySerializer(c.Resolve<IBinarySerializer>(), c.Resolve<IBinaryDeserializer>());
            };
        }

        private static void InitializeCrypto(Crypto crypto)
        {
            Crypto.Initialize(crypto);
        }

        private static void InitializeBinarySerializer(
            IBinarySerializer binarySerializer,
            IBinaryDeserializer binaryDeserializer)
        {
            BinarySerializer.Initialize(binarySerializer);
            BinaryDeserializer.Initialize(binaryDeserializer);
        }
    }
}