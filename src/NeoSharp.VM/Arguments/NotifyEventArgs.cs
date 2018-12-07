using System;

namespace NeoSharp.VM
{
    public class NotifyEventArgs : EventArgs
    {
        /// <summary>
        /// Script Hash
        /// </summary>
        public byte[] ScriptHash { get; private set; }

        /// <summary>
        /// State
        /// </summary>
        public StackItemBase State { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scriptHash">Script hash</param>
        /// <param name="state">State</param>
        public NotifyEventArgs(byte[] scriptHash, StackItemBase state)
        {
            ScriptHash = scriptHash;
            State = state;
        }
    }
}