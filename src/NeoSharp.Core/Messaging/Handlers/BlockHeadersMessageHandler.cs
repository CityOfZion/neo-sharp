using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NeoSharp.Core.Blockchain;
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
            _blockchain.AddBlockHeaders(message.Payload.Headers);

            if (_blockchain.BlockHeaderHeight < sender.Version.StartHeight)
            {
                _logger.LogInformation($"The peer start height is {sender.Version.StartHeight} but the current start height is {_blockchain.BlockHeaderHeight}");
                await sender.Send(new GetBlockHeadersMessage(_blockchain.CurrentBlockHeaderHash));
            }
        }
    }
}