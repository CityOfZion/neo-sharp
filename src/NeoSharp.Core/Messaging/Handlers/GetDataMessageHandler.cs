using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class GetDataMessageHandler : IMessageHandler<GetDataMessage>
    {
        #region Variables

        private readonly IBroadcast _broadcast;
        private readonly ILogger<GetDataMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcast">Broadcast</param>
        /// <param name="logger">Logger</param>
        public GetDataMessageHandler(IBroadcast broadcast, ILogger<GetDataMessageHandler> logger)
        {
            _broadcast = broadcast ?? throw new ArgumentNullException(nameof(broadcast));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handle GetData message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">sender Peer</param>
        /// <returns>Task</returns>
        public async Task Handle(GetDataMessage message, IPeer sender)
        {
            // TODO: do logic

            foreach (UInt256 hash in message.Payload.Hashes.Distinct())
            {
                //IInventory inventory;
                //if (!localNode.RelayCache.TryGet(hash, out inventory) && !localNode.ServiceEnabled)
                //    continue;

                switch (message.Payload.Type)
                {
                    case InventoryType.Tx:
                        {
                            //if (inventory == null)
                            //    inventory = LocalNode.GetTransaction(hash);
                            //if (inventory == null && Blockchain.Default != null)
                            //    inventory = Blockchain.Default.GetTransaction(hash);
                            //if (inventory != null)
                            //    EnqueueMessage("tx", inventory);
                            break;
                        }
                    case InventoryType.Block:
                        {
                            //if (inventory == null && Blockchain.Default != null)
                            //    inventory = Blockchain.Default.GetBlock(hash);

                            //if (inventory != null)
                            //{
                            //    BloomFilter filter = bloom_filter;
                            //    if (filter == null)
                            //    {
                            //        EnqueueMessage("block", inventory);
                            //    }
                            //    else
                            //    {
                            //        Block block = (Block)inventory;
                            //        BitArray flags = new BitArray(block.Transactions.Select(p => TestFilter(filter, p)).ToArray());
                            //        EnqueueMessage("merkleblock", MerkleBlockPayload.Create(block, flags));
                            //    }
                            //}
                            break;
                        }
                    case InventoryType.Consensus:
                        {
                            //if (inventory != null)
                            //    EnqueueMessage("consensus", inventory);
                            break;
                        }
                }
            }

            await Task.CompletedTask;
        }
    }
}