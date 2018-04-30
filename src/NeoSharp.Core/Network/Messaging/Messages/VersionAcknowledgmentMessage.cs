using NeoSharp.Core.Network.Messaging;

namespace NeoSharp.Core.Network.Messages
{
    public class VersionAcknowledgmentMessage : Message
    {
        public VersionAcknowledgmentMessage()
        {
            Command = MessageCommand.verack;
        }
    }
}