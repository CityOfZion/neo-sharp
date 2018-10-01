using NeoSharp.BinarySerialization;
using NeoSharp.Core.DI;
using System;
using System.Linq;

namespace NeoSharp.Application.DI
{
    public class SerializationModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(asm => asm.FullName.StartsWith("Neo"))
                .ToArray();

            containerBuilder.RegisterSingleton<IBinarySerializer>(() => new BinarySerializer(assemblies));

            containerBuilder.OnBuild += c =>
            {
                InitializeBinarySerializer(c.Resolve<IBinarySerializer>());
            };
        }

        private static void InitializeBinarySerializer(IBinarySerializer binarySerializer)
        {
            BinarySerializer.Initialize(binarySerializer);
        }
    }
}