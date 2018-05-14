using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.Interfaces;
using NeoSharp.Core.DI;
using System;
using System.Linq;

namespace NeoSharp.Application.DI
{
    public class SerializationModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IBinarySerializer>(() =>
            {
                var assemblies = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(asm => asm.FullName.StartsWith("Neo"))
                    .ToArray();

                return new BinarySerializer(assemblies);
            });

            containerBuilder.RegisterSingleton<IBinaryDeserializer>(() =>
            {
                var assemblies = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(asm => asm.FullName.StartsWith("Neo"))
                    .ToArray();

                return new BinaryDeserializer(assemblies);
            });
        }
    }
}