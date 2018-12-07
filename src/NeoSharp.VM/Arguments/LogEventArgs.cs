using System;

namespace NeoSharp.VM
{
    public class LogEventArgs : EventArgs
    {
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
        /// <param name="scriptHash">Script hash</param>
        /// <param name="message">Message</param>
        public LogEventArgs(byte[] scriptHash, string message)
        {
            ScriptHash = scriptHash;
            Message = message;
        }
    }
}