using System;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.DI;

namespace NeoSharp.Application.DI
{
    public class SerializationModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterInstanceCreator<IBinarySerializer>(() =>
            {
                var assemblies = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(asm => asm.FullName.StartsWith("Neo"))
                    .ToArray();

                return new BinarySerializer(assemblies);
            });
        }
    }
}