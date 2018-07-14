using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class MemPoolMessageHandler : IMessageHandler<MemPoolMessage>
    {
        #region Variables

        private readonly IBlockchain _blockchain;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockchain">Blockchain</param>
        public MemPoolMessageHandler(IBlockchain blockchain)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
        }

        /// <summary>
        /// Handle GetMemPool message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">Sender</param>
        /// <returns>Task</returns>
        public async Task Handle(MemPoolMessage message, IPeer sender)
        {
            var hashes = _blockchain.MemoryPool
                .Peek(InventoryPayload.MaxHashes)
                .Select(tx => tx.Value.Hash)
                .ToArray();

            await sender.Send(new InventoryMessage(InventoryType.Transaction, hashes));
        }
    }
}