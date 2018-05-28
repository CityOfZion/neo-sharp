using System;
using System.Linq;
using NeoSharp.Core.Messaging;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.ExtensionMethods
{
    public static class MessageExtensionMethods
    {
        private static readonly Type[] _handshakeMessageTypes =
        {
            typeof(VersionMessage),
            typeof(VerAckMessage)
        };

        public static bool IsHandshakeMessage(this Message message)
        {
            return _handshakeMessageTypes.Contains(message.GetType());
        }
    }
}
