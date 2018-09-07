using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class MemPoolMessageHandler : IMessageHandler<MemPoolMessage>
    {
        #region Variables

        private readonly ITransactionPool _transactionPool;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="transactionPool">Transaction Pool</param>
        public MemPoolMessageHandler(ITransactionPool transactionPool)
        {
            _transactionPool = transactionPool ?? throw new ArgumentNullException(nameof(transactionPool));
        }

        /// <summary>
        /// Handle GetMemPool message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">Sender</param>
        /// <returns>Task</returns>
        public async Task Handle(MemPoolMessage message, IPeer sender)
        {
            var hashes = _transactionPool
                .Take(InventoryPayload.MaxHashes)
                .Select(tx => tx.Hash)
                .ToArray();

            await sender.Send(new InventoryMessage(InventoryType.Transaction, hashes));
        }
    }
}