using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class TransactionMessage : Message<Transaction>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TransactionMessage()
        {
            Command = MessageCommand.tx;
            Payload = new Transaction();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="payload">Payload</param>
        public TransactionMessage(Transaction payload)
        {
            Command = MessageCommand.tx;
            Payload = payload;
        }
    }
}