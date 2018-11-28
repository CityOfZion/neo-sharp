using System;

namespace NeoSharp.Core.VM
{
    public interface IInteropService
    {
        void RegisterMethod(string name, Func<IStackAccessor, bool> handler);
    }
}