using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Types;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class GetDataMessageHandler : MessageHandler<GetDataMessage>
    {
        #region Private fields 
        private readonly IBlockRepository _blockRepository;
        private readonly ITransactionRepository _transactionModel;
        private readonly ILogger<GetDataMessageHandler> _logger;
        #endregion

        #region Constructor 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockRepository">The block model.</param>
        /// <param name="transactionModel">The transaction model.</param>
        /// <param name="logger">Logger</param>
        public GetDataMessageHandler(
            IBlockRepository blockRepository, 
            ITransactionRepository transactionModel, 
            ILogger<GetDataMessageHandler> logger)
        {
            // TODO #434: Title not aligned but the context is the same.

            _blockRepository = blockRepository;
            _transactionModel = transactionModel ?? throw new ArgumentNullException(nameof(transactionModel));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region IMessageHandler orveride Methods
        /// <inheritdoc />
        public override async Task Handle(GetDataMessage message, IPeer sender)
        {
            var hashes = message.Payload.Hashes
                .Distinct()
                .ToArray();

            // TODO #376: support local relay cache

            var inventoryType = message.Payload.Type;

            switch (inventoryType)
            {
                case InventoryType.Transaction:
                    {
                        await SendTransactions(hashes, sender);
                        break;
                    }

                case InventoryType.Block:
                    {
                        await SendBlocks(hashes, sender);
                        break;
                    }

                case InventoryType.Consensus:
                    {
                        // TODO #377: Implement after consensus
                        break;
                    }

                default:
                    {
                        _logger.LogError($"The payload of {nameof(InventoryMessage)} contains unknown {nameof(InventoryType)} \"{inventoryType}\".");
                        break;
                    }
            }
        }

        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is GetDataMessage;
        }
        #endregion

        #region Private Methods 
        private async Task SendTransactions(IReadOnlyCollection<UInt256> transactionHashes, IPeer peer)
        {
            var transactions = await _transactionModel.GetTransactions(transactionHashes);

            // TODO #378: The more efficient operation would be to send many transactions per one message
            // but it breaks backward compatibility
            await Task.WhenAll(transactions.Select(t => peer.Send(new TransactionMessage(t))));
        }

        private async Task SendBlocks(IReadOnlyCollection<UInt256> blockHashes, IPeer peer)
        {
            var blocks = (await this._blockRepository.GetBlocks(blockHashes)).ToList();

            if (!blocks.Any()) return;

            var filter = peer.BloomFilter;

            if (filter == null)
            {
                // TODO #378: The more efficient operation would be to send many blocks per one message
                // but it breaks backward compatibility
                await Task.WhenAll(blocks.Select(b => peer.Send(new BlockMessage(b))));
            }
            else
            {
                var merkleBlocks = blocks
                    .ToDictionary(
                        b => b,
                        b => new BitArray(b.Transactions
                            .Select(tx => TestFilter(filter, tx))
                            .ToArray()
                        )
                    );

                // TODO #379: Why don't we have this message?
                // await peer.Send(new MerkleBlockMessage(merkleBlocks));
            }
        }

        private static bool TestFilter(BloomFilter filter, Transaction tx)
        {
            // TODO #380: encapsulate this in filter

            return false;
        }
        #endregion
    }
}