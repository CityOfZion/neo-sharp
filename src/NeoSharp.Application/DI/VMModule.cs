using NeoSharp.Core.DI;
using NeoSharp.Core.SmartContract;
using NeoSharp.VM;
using NeoSharp.VM.Interop;

namespace NeoSharp.Application.DI
{
    public class VMModule : IModule
    {
        public void Register(IContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterSingleton<IVMFactory, NeoVM>();
            
            if (!NeoVM.TryLoadLibrary(out var error))
            {
                // TODO: Log error here
            }

            containerBuilder.RegisterSingleton<InteropService>();
            containerBuilder.RegisterSingleton<IScriptTable, ScriptTable>();
            containerBuilder.Register<IMessageContainer, MessageContainer>();
            containerBuilder.RegisterInstance(new ExecutionEngineLogger(ELogVerbosity.None));
        }
    }
}
