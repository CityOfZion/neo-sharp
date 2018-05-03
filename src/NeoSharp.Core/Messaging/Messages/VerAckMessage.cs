namespace NeoSharp.Core.Messaging.Messages
{
    public class VerAckMessage : Message
    {
        public VerAckMessage()
        {
            Command = MessageCommand.verack;
        }
    }
}