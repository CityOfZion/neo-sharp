using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NeoSharp.Core.Exceptions;
using NeoSharp.Core.Helpers;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManger;
using NeoSharp.Core.Persistence;
using NeoSharp.Types;

namespace NeoSharp.Core.Blockchain.Processing
{
    public class TransactionProcessor
    {
        private static readonly TimeSpan DefaultTransactionPollingInterval = TimeSpan.FromMilliseconds(100);

        private readonly ConcurrentDictionary<UInt256, Transaction> _unverifiedTransactionPool = new ConcurrentDictionary<UInt256, Transaction>();
        private readonly ITransactionPool _verifiedTransactionPool;
        private readonly IRepository _repository;
        private readonly IAsyncDelayer _asyncDelayer;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly IVerifier<Transaction> _transactionVerifier;

        public event EventHandler<Transaction> OnTransactionProcessed;

        public TransactionProcessor(
            ITransactionPool transactionPool,
            IVerifier<Transaction> transactionVerifier,
            IRepository repository,
            IAsyncDelayer asyncDelayer)
        {
            _verifiedTransactionPool = transactionPool ?? throw new ArgumentNullException(nameof(transactionPool));
            _transactionVerifier = transactionVerifier ?? throw new ArgumentNullException(nameof(transactionVerifier));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _asyncDelayer = asyncDelayer ?? throw new ArgumentNullException(nameof(asyncDelayer));
        }

        public void Run()
        {
            var cancellationToken = _cancellationTokenSource.Token;

            Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!_unverifiedTransactionPool.Any())
                    {
                        await _asyncDelayer.Delay(DefaultTransactionPollingInterval, cancellationToken);
                        continue;
                    }

                    var unverifiedTransactionHashes = _unverifiedTransactionPool.Keys;
                    var transactionPool = _verifiedTransactionPool.Concat(
                        _unverifiedTransactionPool.Values.Where(t => unverifiedTransactionHashes.Contains(t.Hash)))
                        .ToArray();

                    foreach (var transactionHash in unverifiedTransactionHashes)
                    {
                        if (!_unverifiedTransactionPool.TryGetValue(transactionHash, out var transaction))
                        {
                            continue;
                        }

                        var valid = this._transactionVerifier.Verify(transaction);
                        
                        if (transactionPool
                            .Where(t => t.Hash != transactionHash)
                            .Where(p => p != transaction)
                            .SelectMany(p => p.Inputs)
                            .Intersect(transaction.Inputs)
                            .Any())
                        {
                            valid = false;
                        }

                        if (valid)
                        {
                            _verifiedTransactionPool.Add(transaction);
                        }

                        _unverifiedTransactionPool.TryRemove(transactionHash, out _);

                        OnTransactionProcessed?.Invoke(this, transaction);
                    }
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task AddTransaction(Transaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            if (_unverifiedTransactionPool.ContainsKey(transaction.Hash))
            {
                throw new InvalidTransactionException($"The transaction  \"{transaction.Hash.ToString(true)}\" was already queued and still not verified to be added.");
            }

            if (_verifiedTransactionPool.Contains(transaction.Hash))
            {
                throw new InvalidTransactionException($"The transaction  \"{transaction.Hash.ToString(true)}\" was already queued and verified to be added.");
            }

            if (await _repository.GetTransaction(transaction.Hash) != null)
            {
                throw new InvalidTransactionException($"The transaction \"{transaction.Hash.ToString(true)}\" exists already on the blockchain.");
            }

            if (!_unverifiedTransactionPool.TryAdd(transaction.Hash, transaction))
            {
                throw new InvalidTransactionException($"The transaction  \"{transaction.Hash.ToString(true)}\" was already queued to be added.");
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }
    }
}