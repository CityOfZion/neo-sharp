using System;

namespace NeoSharp.VM
{
    public class NotifyEventArgs : EventArgs
    {
        /// <summary>
        /// Message Provider
        /// </summary>
        public readonly IMessageProvider MessageProvider;

        /// <summary>
        /// Script Hash
        /// </summary>
        public readonly byte[] ScriptHash;

        /// <summary>
        /// State
        /// </summary>
        public readonly IStackItem State;

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