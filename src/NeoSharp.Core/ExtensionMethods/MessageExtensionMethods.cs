using NeoSharp.Core.Messaging;

namespace NeoSharp.Core.ExtensionMethods
{
    public static class MessageExtensionMethods
    {
        /// <summary>
        /// Is this message a handshake message?
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Return true or false</returns>
        public static bool IsHandshakeMessage(this Message message)
        {
            return message.Command == MessageCommand.verack || message.Command == MessageCommand.version;
        }
    }
}