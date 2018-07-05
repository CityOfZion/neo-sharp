using System;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class BlockHeadersMessageHandler : IMessageHandler<BlockHeadersMessage>
    {
        private readonly IBlockchain _blockchain;
        private readonly ILogger<BlockHeadersMessageHandler> _logger;

        public BlockHeadersMessageHandler(IBlockchain blockchain, ILogger<BlockHeadersMessageHandler> logger)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(BlockHeadersMessage message, IPeer sender)
        {
            _blockchain.AddBlockHeaders(message.Payload.Headers.Select(u => u.Header));

            if (_blockchain.LastBlockHeader.Index < sender.Version.CurrentBlockIndex)
            {
                _logger.LogInformation($"The peer has {sender.Version.CurrentBlockIndex + 1} blocks but the current number of block headers is {_blockchain.LastBlockHeader.Index + 1}.");
                await sender.Send(new GetBlockHeadersMessage(_blockchain.LastBlockHeader.Hash));
            }
        }
    }
}