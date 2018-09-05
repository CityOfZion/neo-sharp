using System;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processors;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class TransactionMessageHandler : IMessageHandler<TransactionMessage>
    {
        #region Variables

        private readonly IBlockchain _blockchain;
        private readonly ITransactionPool _transactionPool;
        private readonly ILogger<TransactionMessageHandler> _logger;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockchain">Blockchain</param>
        /// <param name="transactionPool">Transaction Pool</param>
        /// <param name="logger">Logger</param>
        public TransactionMessageHandler(IBlockchain blockchain, ITransactionPool transactionPool, ILogger<TransactionMessageHandler> logger)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _transactionPool = transactionPool ?? throw new ArgumentNullException(nameof(transactionPool));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(TransactionMessage message, IPeer sender)
        {
            var transaction = message.Payload;

            if (transaction is MinerTransaction) return;

            // TODO: check if the hash of the transaction is known already

            transaction.UpdateHash();

            var transactionExists = await _blockchain.ContainsTransaction(transaction.Hash);
            if (transactionExists)
            {
                _logger.LogInformation($"The transaction \"{transaction.Hash.ToString(true)}\" exists already on the blockchain.");
                return;
            }

            // Transaction is not added right away but queued to be verified and added.
            // It is the reason why we do not broadcast immediately.

            // TODO: It is a bit more complicated

            _transactionPool.Add(transaction);

            var transactionAdded = true;

            if (!transactionAdded)
            {
                _logger.LogWarning($"The transaction \"{transaction.Hash.ToString(true)}\" was not added to the blockchain.");
                return;
            }
        }
    }
}