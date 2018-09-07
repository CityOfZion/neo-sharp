using System;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class BlockMessageHandler : IMessageHandler<BlockMessage>
    {
        #region Variables

        private readonly IBlockProcessor _blockProcessor;
        private readonly IBroadcaster _broadcaster;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockProcessor">Block Pool</param>
        /// <param name="broadcaster">Broadcaster</param>
        public BlockMessageHandler(IBlockProcessor blockProcessor, IBroadcaster broadcaster)
        {
            _blockProcessor = blockProcessor ?? throw new ArgumentNullException(nameof(blockProcessor));
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
        }

        public async Task Handle(BlockMessage message, IPeer sender)
        {
            var block = message.Payload;

            await _blockProcessor.AddBlock(block);

            _broadcaster.Broadcast(message, sender);
        }
    }
}
