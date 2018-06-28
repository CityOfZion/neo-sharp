using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Extensions
{
    public static class MessageExtensions
    {
        /// <summary>
        /// Is this message a handshake message?
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Return true or false</returns>
        public static bool IsHandshakeMessage(this Message message)
        {
            return message is VersionMessage || message is VerAckMessage;
        }
    }
}