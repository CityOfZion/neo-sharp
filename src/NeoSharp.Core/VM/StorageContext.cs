using NeoSharp.Types;

namespace NeoSharp.Core.VM
{
    internal class StorageContext
    {
        public UInt160 ScriptHash;
        public bool IsReadOnly;

        public byte[] ToArray()
        {
            return ScriptHash.ToArray();
        }
    }
}