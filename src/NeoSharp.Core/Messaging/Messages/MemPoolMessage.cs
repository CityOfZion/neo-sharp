namespace NeoSharp.Core.Messaging.Messages
{
    public class MemPoolMessage : Message
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MemPoolMessage()
        {
            Command = MessageCommand.mempool;
        }
    }
}