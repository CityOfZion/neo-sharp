namespace NeoSharp.Core.Messaging.Messages
{
    public class MemPoolMessage : Message
    {
        public MemPoolMessage()
        {
            Command = MessageCommand.mempool;
        }
    }
}