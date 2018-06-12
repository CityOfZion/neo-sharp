using System;
using System.Threading.Tasks;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging
{
    public abstract class InventoryMessageHandler<TMessage> : IMessageHandler<TMessage> where TMessage : Message
    {
        #region Variables

        protected readonly IBroadcast _broadcast;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcast">Broadcast</param>
        protected InventoryMessageHandler(IBroadcast broadcast)
        {
            _broadcast = broadcast ?? throw new ArgumentNullException(nameof(broadcast));
        }

        /// <summary>
        /// Handle Message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">sender Peer</param>
        /// <returns>Task</returns>
        public virtual async Task Handle(TMessage message, IPeer sender)
        {
            /*
            lock (missions_global)
            {
                lock (missions)
                {
                    missions_global.Remove(inventory.Hash);
                    missions.Remove(inventory.Hash);
                    if (missions.Count == 0)
                        mission_start = DateTime.Now.AddYears(100);
                    else
                        mission_start = DateTime.Now;
                }
            }
            if (inventory is MinerTransaction) return;
            InventoryReceived?.Invoke(this, inventory);
            */

            // Send message to every one except the sender ?

            // await _broadcast.SendBroadcast(message, (peer) => peer != sender);

            /*
            if (inventory is Transaction tx && tx.Type != TransactionType.ClaimTransaction && tx.Type != TransactionType.IssueTransaction)
            {
                if (Blockchain.Default == null) return;
                if (!CheckKnownHashes(inventory.Hash)) return;
                InventoryReceivingEventArgs args = new InventoryReceivingEventArgs(inventory);
                InventoryReceiving?.Invoke(this, args);
                if (args.Cancel) return;
                lock (temp_pool)
                {
                    temp_pool.Add(tx);
                }
                new_tx_event.Set();
            }
            else
            {
                Relay(inventory);
            }
            */

            await Task.CompletedTask;
        }
    }
}