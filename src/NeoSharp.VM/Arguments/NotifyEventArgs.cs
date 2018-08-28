using System;

namespace NeoSharp.VM
{
    public class NotifyEventArgs : EventArgs
    {
        /// <summary>
        /// Message Provider
        /// </summary>
        public IMessageProvider MessageProvider { get; private set; }

        /// <summary>
        /// Script Hash
        /// </summary>
        public byte[] ScriptHash { get; private set; }

        /// <summary>
        /// State
        /// </summary>
        public IStackItem State { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageProvider">Message Provider</param>
        /// <param name="scriptHash">Script hash</param>
        /// <param name="state">State</param>
        public NotifyEventArgs(IMessageProvider messageProvider, byte[] scriptHash, IStackItem state)
        {
            MessageProvider = messageProvider;
            ScriptHash = scriptHash;
            State = state;
        }
    }
}