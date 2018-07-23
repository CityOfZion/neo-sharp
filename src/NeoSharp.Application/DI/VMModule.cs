using NeoSharp.Core.DI;
using NeoSharp.VM;
using NeoSharp.VM.Interop;

namespace NeoSharp.Application.DI
{
    public class VMModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            // TODO: Require set VM native dll on environment variable

            //containerBuilder.RegisterSingleton<IVMFactory, NeoVM>();
        }
    }
}