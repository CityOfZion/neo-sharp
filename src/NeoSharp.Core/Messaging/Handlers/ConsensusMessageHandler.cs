using System;
using System.Threading.Tasks;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class ConsensusMessageHandler : MessageHandler<ConsensusMessage>
    {
        #region Private fields 
        private readonly IBroadcaster _broadcaster;
        #endregion

        #region Constructor 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="broadcaster">Broadcaster</param>
        public ConsensusMessageHandler(IBroadcaster broadcaster)
        {
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
        }
        #endregion

        #region MessageHandler override methods
        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is ConsensusMessage;
        }

        /// <inheritdoc />
        public override Task Handle(ConsensusMessage message, IPeer sender)
        {
            _broadcaster.Broadcast(message, sender);

            return Task.CompletedTask;
        }
        #endregion
    }
}