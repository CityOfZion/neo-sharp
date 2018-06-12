using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Messaging.Messages
{
    public class TxMessage : Message<TxPayload>
    {
        public TxMessage()
        {
            Command = MessageCommand.tx;
            Payload = new TxPayload();
        }

        public TxMessage(Transaction tx)
        {
            Command = MessageCommand.tx;
            Payload = new TxPayload { Tx = tx };
        }
    }

    public class TxPayload
    {
        [BinaryProperty(0)]
        public Transaction Tx;
    }
}