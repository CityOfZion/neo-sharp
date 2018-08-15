using System;
using Newtonsoft.Json;

namespace NeoSharp.VM.Interop.Native
{
    public interface INativeStackItem
    {
        /// <summary>
        /// Handle
        /// </summary>
        [JsonIgnore]
        IntPtr Handle { get; }

        /// <summary>
        /// Type
        /// </summary>
        EStackItemType Type { get; }

        /// <summary>
        /// Get native byte array
        /// </summary>
        byte[] GetNativeByteArray();
    }
}