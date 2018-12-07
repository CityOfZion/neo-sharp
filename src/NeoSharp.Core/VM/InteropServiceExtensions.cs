using System;
using NeoSharp.VM;

namespace NeoSharp.Core.VM
{
    public static class InteropServiceExtensions
    {
        public static void RegisterStackTransition(
            this InteropService interopService,
            string name,
            Func<IStackAccessor, bool> handler)
        {
            bool BaseHandler(ExecutionEngineBase engine)
            {
                var ctx = engine.CurrentContext;
                if (ctx == null) return false;

                return handler(new StackAccessor(engine, ctx));
            }

            interopService.Register(name, BaseHandler);
        }
    }
}