using System;
using System.Threading.Tasks;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging
{
    public abstract class BroadcastMessageHandler<TMessage> : IMessageHandler<TMessage> where TMessage : Message
    {
        #region Variables

        protected readonly IBroadcast _broadcast;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcast">Broadcast</param>
        protected BroadcastMessageHandler(IBroadcast broadcast)
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
            // Send message to every one except the sender

            await _broadcast.SendBroadcast(message, (peer) => peer != sender);
        }
    }
}