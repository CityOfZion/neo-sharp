using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class TransactionMessage : Message<Transaction>
    {
        public TransactionMessage()
        {
            Command = MessageCommand.tx;
            Payload = new Transaction();
        }

        public TransactionMessage(Transaction transaction)
        {
            Command = MessageCommand.tx;
            Payload = transaction;
        }
    }
}