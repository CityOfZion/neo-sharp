using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class MemPoolMessageHandler : MessageHandler<MemPoolMessage>
    {
        #region Private fields 
        private readonly ITransactionPool _transactionPool;
        #endregion

        #region Constructor 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transactionPool">Transaction Pool</param>
        public MemPoolMessageHandler(ITransactionPool transactionPool)
        {
            _transactionPool = transactionPool ?? throw new ArgumentNullException(nameof(transactionPool));
        }
        #endregion

        #region MessageHandler override methods
        /// <inheritdoc />
        public override async Task Handle(MemPoolMessage message, IPeer sender)
        {
            var hashes = _transactionPool
                .Take(InventoryPayload.MaxHashes)
                .Select(tx => tx.Hash)
                .ToArray();

            await sender.Send(new InventoryMessage(InventoryType.Transaction, hashes));
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is MemPoolMessage;
        }
        #endregion
    }
}