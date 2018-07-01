using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class InventoryMessageHandler : IMessageHandler<InventoryMessage>
    {
        #region Variables

        private readonly IBlockchain _blockchain;
        private readonly ILogger<InventoryMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockchain">Blockchain</param>
        /// <param name="logger">Logger</param>
        public InventoryMessageHandler(
            IBlockchain blockchain,
            ILogger<InventoryMessageHandler> logger)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handle Inventory message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="sender">Sender</param>
        /// <returns>Task</returns>
        public async Task Handle(InventoryMessage message, IPeer sender)
        {
            var hashes = message.Payload.Hashes
                .Distinct()
                .ToArray();

            // TODO: exclude known hashes

            if (hashes.Any() == false)
            {
                _logger.LogWarning("The payload of InvetoryMessage contains no hashes.");
                return;
            }

            // TODO: support local relay cache

            switch (message.Payload.Type)
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
                        // TODO: Implement after consensus
                        break;
                    }

                default:
                    _logger.LogError(
                        $"The payload of InvetoryMessage contains unknown inventory type \"{message.Payload.Type}\".");
                    break;
            }
        }

        private async Task SendTransactions(IReadOnlyCollection<UInt256> transactionHashes, IPeer peer)
        {
            var transactions = _blockchain.GetTransactions(transactionHashes);

            await peer.Send(new TransactionMessage(transactions));
        }

        private async Task SendBlocks(IReadOnlyCollection<UInt256> blockHashes, IPeer peer)
        {
            var blocks = _blockchain.GetBlocks(blockHashes);

            var filter = peer.BloomFilter;
            if (filter == null)
            {
                await peer.Send(new BlockMessage(blocks));
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

                // TODO: Why don't we have this message?
                // await peer.Send(new MerkleBlockMessage(merkleBlocks));
            }
        }

        private bool TestFilter(BloomFilter filter, Transaction tx)
        {
            // TODO: encapsulate this in filter

            return false;
        }

    }
}