using System;

namespace NeoSharp.VM
{
    public class LogEventArgs : EventArgs
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
        /// Message
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageProvider">Message Provider</param>
        /// <param name="scriptHash">Script hash</param>
        /// <param name="message">Message</param>
        public LogEventArgs(IMessageProvider messageProvider, byte[] scriptHash, string message)
        {
            MessageProvider = messageProvider;
            ScriptHash = scriptHash;
            Message = message;
        }
    }
}