using NeoSharp.Core.DI;
using NeoSharp.VM;

namespace NeoSharp.Application.DI
{
    public class VMModule : IModule
    {
        class NullVM : IVMFactory
        {
            public IExecutionEngine Create(ExecutionEngineArgs args)
            {
                throw new System.NotImplementedException();
            }
        }

        public void Register(IContainerBuilder containerBuilder)
        {
            // TODO: Inject here HyperVM Nuget package

            containerBuilder.RegisterSingleton<IVMFactory, NullVM>();
            //containerBuilder.RegisterSingleton<IVMFactory, NeoVM>();
        }
    }
}