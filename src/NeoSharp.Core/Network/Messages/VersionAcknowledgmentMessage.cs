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