using System;
using NeoSharp.VM;

namespace NeoSharp.Core.VM
{
    public static class InteropServiceExtensions
    {
        public static void RegisterStackMethod(
            this InteropService interopService,
            string name,
            Func<Stack, bool> handler)
        {
            bool InvokeHandler(ExecutionEngineBase engine)
            {
                return engine.CurrentContext != null && handler(engine.CurrentContext.EvaluationStack);
            }

            interopService.Register(name, InvokeHandler);
        }
    }
}