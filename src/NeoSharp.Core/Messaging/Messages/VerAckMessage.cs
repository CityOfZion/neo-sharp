namespace NeoSharp.Core.Messaging.Messages
{
    public class VerAckMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public VerAckMessage()
        {
            Command = MessageCommand.verack;
        }
    }
}