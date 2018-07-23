using NeoSharp.VM.Interop.Enums;

namespace NeoSharp.VM.Interop.Native
{
    internal class MacCore : LinuxCore
    {
        public MacCore() : base(EPlatform.Mac, ".so") { }
    }
}