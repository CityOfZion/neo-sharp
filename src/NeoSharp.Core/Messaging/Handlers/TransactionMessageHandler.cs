using System;
using System.Threading.Tasks;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Logging;
using NeoSharp.Core.Messaging.Messages;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Messaging.Handlers
{
    public class TransactionMessageHandler : MessageHandler<TransactionMessage>
    {
        #region Private Fields 
        private readonly IBlockchain _blockchain;
        private readonly ITransactionPool _transactionPool;
        private readonly ITransactionSigner _transactionSigner;
        private readonly ILogger<TransactionMessageHandler> _logger;
        #endregion

        #region Constructor 
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="blockchain">Blockchain</param>
        /// <param name="transactionPool">Transaction Pool</param>
        /// <param name="transactionSigner">The transaction operation manager</param>
        /// <param name="logger">Logger</param>
        public TransactionMessageHandler(
            IBlockchain blockchain, 
            ITransactionPool transactionPool, 
            ITransactionSigner transactionSigner,
            ILogger<TransactionMessageHandler> logger)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));
            _transactionPool = transactionPool ?? throw new ArgumentNullException(nameof(transactionPool));
            _transactionSigner = transactionSigner;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region MessageHandler override methods
        /// <inheritdoc />
        public override bool CanHandle(Message message)
        {
            return message is TransactionMessage;
        }

        /// <inheritdoc />
        public override async Task Handle(TransactionMessage message, IPeer sender)
        {
            var transaction = message.Payload;

            if (transaction is MinerTransaction) return;

            // TODO #373: check if the hash of the transaction is known already
            if (transaction.Hash == null)
            {
                this._transactionSigner.Sign(transaction);
            }

            var transactionExists = await _blockchain.ContainsTransaction(transaction.Hash);
            if (transactionExists)
            {
                _logger.LogInformation($"The transaction \"{transaction.Hash?.ToString(true)}\" exists already on the blockchain.");
                return;
            }

            // Transaction is not added right away but queued to be verified and added.
            // It is the reason why we do not broadcast immediately.

            // TODO #374: It is a bit more complicated

            _transactionPool.Add(transaction);
            _logger.LogInformation($"Transaction with Hash {transaction.Hash?.ToString(true)} added to the TransactionPool.");
        }
        #endregion
    }
}