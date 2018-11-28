using NeoSharp.Core.DI;
using NeoSharp.Core.SmartContract;
using NeoSharp.VM;
using NeoSharp.VM.NeoVM;

namespace NeoSharp.Application.DI
{
    public class VMModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            // HyperVM

            //containerBuilder.RegisterSingleton<IVMFactory, NeoVM>();
            //if (!NeoVM.TryLoadLibrary(out var error))
            //{
            //    // TODO: Log error here
            //}

            containerBuilder.RegisterSingleton<IVMFactory, NeoVMFactory>();
            
            containerBuilder.RegisterSingleton<InteropService>();
            containerBuilder.RegisterSingleton<IScriptTable, ScriptTable>();
            containerBuilder.Register<IMessageContainer, MessageContainer>();
            containerBuilder.RegisterInstance(new ExecutionEngineLogger(ELogVerbosity.None));
        }
    }
}