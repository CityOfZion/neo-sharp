using System;

namespace NeoSharp.VM
{
    [Flags]
    public enum ELogVerbosity : byte
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Enable step into logs
        /// </summary>
        StepInto = 0x01
    }
}