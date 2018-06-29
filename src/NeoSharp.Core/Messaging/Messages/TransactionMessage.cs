using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class TransactionMessage : Message<TransactionPayload>
    {
        public TransactionMessage()
        {
            Command = MessageCommand.tx;
            Payload = new TransactionPayload();
        }

        public TransactionMessage(Transaction transaction)
        {
            Command = MessageCommand.tx;
            Payload = new TransactionPayload { Transactions = new[] { transaction } };
        }

        public TransactionMessage(IEnumerable<Transaction> transactions)
        {
            Command = MessageCommand.tx;
            Payload = new TransactionPayload { Transactions = transactions.ToArray() };
        }
    }

    public class TransactionPayload
    {
        [BinaryProperty(0)]
        public Transaction[] Transactions;
    }
}