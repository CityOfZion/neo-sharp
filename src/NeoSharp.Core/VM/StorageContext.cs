using NeoSharp.Types;

namespace NeoSharp.Core.VM
{
    public class StorageContext
    {
        /// <summary>
        /// Script hash
        /// </summary>
        public readonly UInt160 ScriptHash;

        /// <summary>
        /// Is readonly?
        /// </summary>
        public readonly bool IsReadOnly;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scriptHash">Script hash</param>
        /// <param name="isReadOnly">Is read only</param>
        public StorageContext(UInt160 scriptHash,bool isReadOnly)
        {
            ScriptHash = scriptHash;
            IsReadOnly = isReadOnly;
        }

        public byte[] ToArray() => ScriptHash.ToArray();
    }
}