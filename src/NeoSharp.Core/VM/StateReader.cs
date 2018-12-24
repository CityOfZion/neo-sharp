using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManager;
using NeoSharp.Core.Network;
using NeoSharp.Types;
using NeoSharp.VM;

namespace NeoSharp.Core.VM
{
    public class StateReader : IDisposable
    {
        // TODO: Move const into app engine
        private const int EngineMaxArraySize = 1024;
        private readonly IBlockchainContext _blockchainContext;
        private readonly IBlockRepository _blockRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionOperationsManager _transactionOperationsManager;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        protected DataCache<UInt160, Account> Accounts { get; }

        protected DataCache<UInt256, Asset> Assets { get; }

        protected DataCache<UInt160, Contract> Contracts { get; }

        protected DataCache<StorageKey, StorageValue> Storages { get; }

        public StateReader(
            DataCache<UInt160, Account> accounts,
            DataCache<UInt256, Asset> assets,
            DataCache<UInt160, Contract> contracts,
            DataCache<StorageKey, StorageValue> storages,
            InteropService interopService,
            IBlockchainContext blockchainContext,
            IBlockRepository blockRepository,
            ITransactionRepository transactionRepository,
            ITransactionOperationsManager transactionOperationsManager)
        {
            Accounts = accounts;
            Assets = assets;
            Contracts = contracts;
            Storages = storages;
            _blockchainContext = blockchainContext;
            _blockRepository = blockRepository;
            _transactionRepository = transactionRepository;
            _transactionOperationsManager = transactionOperationsManager;

            //Standard Library
            interopService.Register("System.Runtime.CheckWitness", Runtime_CheckWitness);
            interopService.Register("System.Runtime.GetTime", Runtime_GetTime);
            interopService.Register("System.Blockchain.GetHeight", Blockchain_GetHeight);
            interopService.Register("System.Blockchain.GetHeader", Blockchain_GetHeader);
            interopService.Register("System.Blockchain.GetBlock", Blockchain_GetBlock);
            interopService.Register("System.Blockchain.GetTransaction", Blockchain_GetTransaction);
            interopService.Register("System.Blockchain.GetTransactionHeight", Blockchain_GetTransactionHeight);
            interopService.Register("System.Blockchain.GetContract", Blockchain_GetContract);
            interopService.Register("System.Header.GetIndex", Header_GetIndex);
            interopService.Register("System.Header.GetHash", Header_GetHash);
            interopService.Register("System.Header.GetPrevHash", Header_GetPrevHash);
            interopService.Register("System.Header.GetTimestamp", Header_GetTimestamp);
            interopService.Register("System.Block.GetTransactionCount", Block_GetTransactionCount);
            interopService.Register("System.Block.GetTransactions", Block_GetTransactions);
            interopService.Register("System.Block.GetTransaction", Block_GetTransaction);
            interopService.Register("System.Transaction.GetHash", Transaction_GetHash);
            interopService.Register("System.Storage.GetContext", Storage_GetContext);
            interopService.Register("System.Storage.GetReadOnlyContext", Storage_GetReadOnlyContext);
            interopService.Register("System.Storage.Get", Storage_Get);
            interopService.Register("System.StorageContext.AsReadOnly", StorageContext_AsReadOnly);

            //Neo Specified
            interopService.Register("Neo.Blockchain.GetAccount", Blockchain_GetAccount);
            interopService.Register("Neo.Blockchain.GetValidators", Blockchain_GetValidators);
            interopService.Register("Neo.Blockchain.GetAsset", Blockchain_GetAsset);
            interopService.Register("Neo.Header.GetVersion", Header_GetVersion);
            interopService.Register("Neo.Header.GetMerkleRoot", Header_GetMerkleRoot);
            interopService.Register("Neo.Header.GetConsensusData", Header_GetConsensusData);
            interopService.Register("Neo.Header.GetNextConsensus", Header_GetNextConsensus);
            interopService.Register("Neo.Transaction.GetType", Transaction_GetType);
            interopService.Register("Neo.Transaction.GetAttributes", Transaction_GetAttributes);
            interopService.Register("Neo.Transaction.GetInputs", Transaction_GetInputs);
            interopService.Register("Neo.Transaction.GetOutputs", Transaction_GetOutputs);
            interopService.Register("Neo.Transaction.GetReferences", Transaction_GetReferences);
            interopService.Register("Neo.Transaction.GetUnspentCoins", Transaction_GetUnspentCoins);
            interopService.Register("Neo.InvocationTransaction.GetScript", InvocationTransaction_GetScript);
            interopService.Register("Neo.Attribute.GetUsage", Attribute_GetUsage);
            interopService.Register("Neo.Attribute.GetData", Attribute_GetData);
            interopService.Register("Neo.Input.GetHash", Input_GetHash);
            interopService.Register("Neo.Input.GetIndex", Input_GetIndex);
            interopService.Register("Neo.Output.GetAssetId", Output_GetAssetId);
            interopService.Register("Neo.Output.GetValue", Output_GetValue);
            interopService.Register("Neo.Output.GetScriptHash", Output_GetScriptHash);
            interopService.Register("Neo.Account.GetScriptHash", Account_GetScriptHash);
            interopService.Register("Neo.Account.GetVotes", Account_GetVotes);
            interopService.Register("Neo.Account.GetBalance", Account_GetBalance);
            interopService.Register("Neo.Asset.GetAssetId", Asset_GetAssetId);
            interopService.Register("Neo.Asset.GetAssetType", Asset_GetAssetType);
            interopService.Register("Neo.Asset.GetAmount", Asset_GetAmount);
            interopService.Register("Neo.Asset.GetAvailable", Asset_GetAvailable);
            interopService.Register("Neo.Asset.GetPrecision", Asset_GetPrecision);
            interopService.Register("Neo.Asset.GetOwner", Asset_GetOwner);
            interopService.Register("Neo.Asset.GetAdmin", Asset_GetAdmin);
            interopService.Register("Neo.Asset.GetIssuer", Asset_GetIssuer);
            interopService.Register("Neo.Contract.GetScript", Contract_GetScript);
            interopService.Register("Neo.Contract.IsPayable", Contract_IsPayable);
            interopService.Register("Neo.Storage.Find", Storage_Find);

            #region Old APIs
            interopService.Register("Neo.Runtime.CheckWitness", Runtime_CheckWitness);
            interopService.Register("AntShares.Runtime.CheckWitness", Runtime_CheckWitness);
            interopService.Register("Neo.Runtime.GetTime", Runtime_GetTime);
            interopService.Register("Neo.Blockchain.GetHeight", Blockchain_GetHeight);
            interopService.Register("AntShares.Blockchain.GetHeight", Blockchain_GetHeight);
            interopService.Register("Neo.Blockchain.GetHeader", Blockchain_GetHeader);
            interopService.Register("AntShares.Blockchain.GetHeader", Blockchain_GetHeader);
            interopService.Register("Neo.Blockchain.GetBlock", Blockchain_GetBlock);
            interopService.Register("AntShares.Blockchain.GetBlock", Blockchain_GetBlock);
            interopService.Register("Neo.Blockchain.GetTransaction", Blockchain_GetTransaction);
            interopService.Register("AntShares.Blockchain.GetTransaction", Blockchain_GetTransaction);
            interopService.Register("Neo.Blockchain.GetTransactionHeight", Blockchain_GetTransactionHeight);
            interopService.Register("AntShares.Blockchain.GetAccount", Blockchain_GetAccount);
            interopService.Register("AntShares.Blockchain.GetValidators", Blockchain_GetValidators);
            interopService.Register("AntShares.Blockchain.GetAsset", Blockchain_GetAsset);
            interopService.Register("Neo.Blockchain.GetContract", Blockchain_GetContract);
            interopService.Register("AntShares.Blockchain.GetContract", Blockchain_GetContract);
            interopService.Register("Neo.Header.GetIndex", Header_GetIndex);
            interopService.Register("Neo.Header.GetHash", Header_GetHash);
            interopService.Register("AntShares.Header.GetHash", Header_GetHash);
            interopService.Register("AntShares.Header.GetVersion", Header_GetVersion);
            interopService.Register("Neo.Header.GetPrevHash", Header_GetPrevHash);
            interopService.Register("AntShares.Header.GetPrevHash", Header_GetPrevHash);
            interopService.Register("AntShares.Header.GetMerkleRoot", Header_GetMerkleRoot);
            interopService.Register("Neo.Header.GetTimestamp", Header_GetTimestamp);
            interopService.Register("AntShares.Header.GetTimestamp", Header_GetTimestamp);
            interopService.Register("AntShares.Header.GetConsensusData", Header_GetConsensusData);
            interopService.Register("AntShares.Header.GetNextConsensus", Header_GetNextConsensus);
            interopService.Register("Neo.Block.GetTransactionCount", Block_GetTransactionCount);
            interopService.Register("AntShares.Block.GetTransactionCount", Block_GetTransactionCount);
            interopService.Register("Neo.Block.GetTransactions", Block_GetTransactions);
            interopService.Register("AntShares.Block.GetTransactions", Block_GetTransactions);
            interopService.Register("Neo.Block.GetTransaction", Block_GetTransaction);
            interopService.Register("AntShares.Block.GetTransaction", Block_GetTransaction);
            interopService.Register("Neo.Transaction.GetHash", Transaction_GetHash);
            interopService.Register("AntShares.Transaction.GetHash", Transaction_GetHash);
            interopService.Register("AntShares.Transaction.GetType", Transaction_GetType);
            interopService.Register("AntShares.Transaction.GetAttributes", Transaction_GetAttributes);
            interopService.Register("AntShares.Transaction.GetInputs", Transaction_GetInputs);
            interopService.Register("AntShares.Transaction.GetOutputs", Transaction_GetOutputs);
            interopService.Register("AntShares.Transaction.GetReferences", Transaction_GetReferences);
            interopService.Register("AntShares.Attribute.GetUsage", Attribute_GetUsage);
            interopService.Register("AntShares.Attribute.GetData", Attribute_GetData);
            interopService.Register("AntShares.Input.GetHash", Input_GetHash);
            interopService.Register("AntShares.Input.GetIndex", Input_GetIndex);
            interopService.Register("AntShares.Output.GetAssetId", Output_GetAssetId);
            interopService.Register("AntShares.Output.GetValue", Output_GetValue);
            interopService.Register("AntShares.Output.GetScriptHash", Output_GetScriptHash);
            interopService.Register("AntShares.Account.GetScriptHash", Account_GetScriptHash);
            interopService.Register("AntShares.Account.GetVotes", Account_GetVotes);
            interopService.Register("AntShares.Account.GetBalance", Account_GetBalance);
            interopService.Register("AntShares.Asset.GetAssetId", Asset_GetAssetId);
            interopService.Register("AntShares.Asset.GetAssetType", Asset_GetAssetType);
            interopService.Register("AntShares.Asset.GetAmount", Asset_GetAmount);
            interopService.Register("AntShares.Asset.GetAvailable", Asset_GetAvailable);
            interopService.Register("AntShares.Asset.GetPrecision", Asset_GetPrecision);
            interopService.Register("AntShares.Asset.GetOwner", Asset_GetOwner);
            interopService.Register("AntShares.Asset.GetAdmin", Asset_GetAdmin);
            interopService.Register("AntShares.Asset.GetIssuer", Asset_GetIssuer);
            interopService.Register("AntShares.Contract.GetScript", Contract_GetScript);
            interopService.Register("Neo.Storage.GetContext", Storage_GetContext);
            interopService.Register("AntShares.Storage.GetContext", Storage_GetContext);
            interopService.Register("Neo.Storage.GetReadOnlyContext", Storage_GetReadOnlyContext);
            interopService.Register("Neo.Storage.Get", Storage_Get);
            interopService.Register("AntShares.Storage.Get", Storage_Get);
            interopService.Register("Neo.StorageContext.AsReadOnly", StorageContext_AsReadOnly);
            #endregion
        }

        internal bool CheckStorageContext(StorageContext context)
        {
            var contract = Contracts.TryGet(context.ScriptHash);

            return contract != null && contract.HasStorage;
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
            _disposables.Clear();
        }

        protected bool CheckWitness(IExecutionEngine engine, UInt160 hash)
        {
            var transaction = (InvocationTransaction)engine.MessageProvider.GetMessage(0);
            var hashesForVerifying = _transactionOperationsManager.GetScriptHashes(transaction).Result;
            return hashesForVerifying.Contains(hash);
        }

        protected bool CheckWitness(IExecutionEngine engine, ECPoint pubKey)
        {
            // TODO: Contract.CreateSignatureRedeemScript?
            //return CheckWitness(engine, Contract.CreateSignatureRedeemScript(pubkey).ToScriptHash());
            return true;
        }

        protected virtual bool Runtime_CheckWitness(IExecutionEngine engine)
        {
            bool result;
            var hashOrPubKey = engine.CurrentContext.EvaluationStack.PopByteArray();

            if (hashOrPubKey.Length == 20)
                result = CheckWitness(engine, new UInt160(hashOrPubKey));
            else if (hashOrPubKey.Length == 33)
                result = CheckWitness(engine, new ECPoint(hashOrPubKey));
            else
                return false;

            engine.CurrentContext.EvaluationStack.Push(result);

            return true;
        }

        protected virtual bool Runtime_GetTime(IExecutionEngine engine)
        {
            engine.CurrentContext.EvaluationStack.Push(_blockchainContext.LastBlockHeader.Timestamp + 15);
            return true;
        }

        protected virtual bool Blockchain_GetHeight(IExecutionEngine engine)
        {
            engine.CurrentContext.EvaluationStack.Push(_blockchainContext.CurrentBlock.Index);
            return true;
        }

        protected virtual bool Blockchain_GetHeader(IExecutionEngine engine)
        {
            var data = engine.CurrentContext.EvaluationStack.PopByteArray();

            BlockHeader blockHeader;

            if (data.Length <= 5)
            {
                var height = (uint)new BigInteger(data);

                blockHeader = _blockRepository.GetBlockHeader(height).Result;
            }
            else if (data.Length == 32)
            {
                var hash = new UInt256(data);

                blockHeader = _blockRepository.GetBlockHeader(hash).Result;
            }
            else
            {
                return false;
            }

            engine.CurrentContext.EvaluationStack.PushObject(blockHeader);

            return true;
        }

        protected virtual bool Blockchain_GetBlock(IExecutionEngine engine)
        {
            var data = engine.CurrentContext.EvaluationStack.PopByteArray();

            Block block;

            if (data.Length <= 5)
            {
                var height = (uint)new BigInteger(data);

                block = _blockRepository.GetBlock(height).Result;
            }
            else if (data.Length == 32)
            {
                var hash = new UInt256(data);

                block = _blockRepository.GetBlock(hash).Result;
            }
            else
            {
                return false;
            }

            engine.CurrentContext.EvaluationStack.PushObject(block);

            return true;
        }

        protected virtual bool Blockchain_GetTransaction(IExecutionEngine engine)
        {
            var hash = engine.CurrentContext.EvaluationStack.PopByteArray();
            var transaction = _transactionRepository.GetTransaction(new UInt256(hash)).Result;

            engine.CurrentContext.EvaluationStack.PushObject(transaction);

            return true;
        }

        protected virtual bool Blockchain_GetTransactionHeight(IExecutionEngine engine)
        {
            var hash = engine.CurrentContext.EvaluationStack.PopByteArray();
            // TODO: looks like we need an index transaction in the block;
            var height = 0;
            // var transaction = _transactionRepository.GetTransaction(new UInt256(hash)).Result;

            engine.CurrentContext.EvaluationStack.Push(height);

            return true;
        }

        protected virtual bool Blockchain_GetAccount(IExecutionEngine engine)
        {
            var hash = new UInt160(engine.CurrentContext.EvaluationStack.PopByteArray());
            var account = Accounts.GetOrAdd(hash, () => new Account(hash));

            engine.CurrentContext.EvaluationStack.PushObject(account);

            return true;
        }

        protected virtual bool Blockchain_GetValidators(IExecutionEngine engine)
        {
            // TODO: looks like we need to get all validators
            //ECPoint[] validators = _blockchain.GetValidators();
            //engine.CurrentContext.EvaluationStack.Push(validators.Select(p => (engine.CurrentContext.EvaluationStackItem)p.EncodePoint(true)).ToArray());
            return true;
        }

        protected virtual bool Blockchain_GetAsset(IExecutionEngine engine)
        {
            var hash = new UInt256(engine.CurrentContext.EvaluationStack.PopByteArray());
            var asset = Assets.TryGet(hash);
            if (asset == null) return false;
            engine.CurrentContext.EvaluationStack.PushObject(asset);
            return true;
        }

        protected virtual bool Blockchain_GetContract(IExecutionEngine engine)
        {
            var hash = new UInt160(engine.CurrentContext.EvaluationStack.PopByteArray());
            var contract = Contracts.TryGet(hash);
            if (contract == null)
                engine.CurrentContext.EvaluationStack.Push(Array.Empty<byte>());
            else
                engine.CurrentContext.EvaluationStack.PushObject(contract);
            return true;
        }

        protected virtual bool Header_GetIndex(IExecutionEngine engine)
        {
            var blockBase = engine.CurrentContext.EvaluationStack.PopObject<BlockBase>();
            if (blockBase == null) return false;

            engine.CurrentContext.EvaluationStack.Push(blockBase.Index);

            return true;
        }

        protected virtual bool Header_GetHash(IExecutionEngine engine)
        {
            var blockBase = engine.CurrentContext.EvaluationStack.PopObject<BlockBase>();
            if (blockBase == null) return false;

            engine.CurrentContext.EvaluationStack.Push(blockBase.Hash.ToArray());

            return true;
        }

        protected virtual bool Header_GetVersion(IExecutionEngine engine)
        {
            var blockBase = engine.CurrentContext.EvaluationStack.PopObject<BlockBase>();
            if (blockBase == null) return false;

            engine.CurrentContext.EvaluationStack.Push(blockBase.Version);

            return true;
        }

        protected virtual bool Header_GetPrevHash(IExecutionEngine engine)
        {
            var blockBase = engine.CurrentContext.EvaluationStack.PopObject<BlockBase>();
            if (blockBase == null) return false;

            engine.CurrentContext.EvaluationStack.Push(blockBase.PreviousBlockHash.ToArray());

            return true;
        }

        protected virtual bool Header_GetMerkleRoot(IExecutionEngine engine)
        {
            // TODO: Should MerkleRoot be in BlockBase?
            var blockBase = engine.CurrentContext.EvaluationStack.PopObject<BlockHeader>();
            if (blockBase == null) return false;

            engine.CurrentContext.EvaluationStack.Push(blockBase.MerkleRoot.ToArray());

            return true;
        }

        protected virtual bool Header_GetTimestamp(IExecutionEngine engine)
        {
            var blockBase = engine.CurrentContext.EvaluationStack.PopObject<BlockBase>();
            if (blockBase == null) return false;

            engine.CurrentContext.EvaluationStack.Push(blockBase.Timestamp);

            return true;
        }

        protected virtual bool Header_GetConsensusData(IExecutionEngine engine)
        {
            var blockBase = engine.CurrentContext.EvaluationStack.PopObject<BlockBase>();
            if (blockBase == null) return false;

            engine.CurrentContext.EvaluationStack.Push(blockBase.ConsensusData);

            return true;
        }

        protected virtual bool Header_GetNextConsensus(IExecutionEngine engine)
        {
            var blockBase = engine.CurrentContext.EvaluationStack.PopObject<BlockBase>();
            if (blockBase == null) return false;

            engine.CurrentContext.EvaluationStack.Push(blockBase.NextConsensus.ToArray());

            return true;
        }

        protected virtual bool Block_GetTransactionCount(IExecutionEngine engine)
        {
            var block = engine.CurrentContext.EvaluationStack.PopObject<Block>();
            if (block == null) return false;

            engine.CurrentContext.EvaluationStack.Push(block.Transactions.Length);

            return true;
        }

        protected virtual bool Block_GetTransactions(IExecutionEngine engine)
        {
            var block = engine.CurrentContext.EvaluationStack.PopObject<Block>();
            if (block == null) return false;
            if (block.Transactions.Length > EngineMaxArraySize) return false;

            engine.CurrentContext.EvaluationStack.PushArray(block.Transactions);

            return true;
        }

        protected virtual bool Block_GetTransaction(IExecutionEngine engine)
        {
            var block = engine.CurrentContext.EvaluationStack.PopObject<Block>();
            if (block == null) return false;

            var index = (int)engine.CurrentContext.EvaluationStack.PopBigInteger();
            if (index < 0 || index >= block.Transactions.Length) return false;

            var transaction = block.Transactions[index];

            engine.CurrentContext.EvaluationStack.PushObject(transaction);

            return true;
        }

        protected virtual bool Transaction_GetHash(IExecutionEngine engine)
        {
            var transaction = engine.CurrentContext.EvaluationStack.PopObject<Transaction>();
            if (transaction == null) return false;

            engine.CurrentContext.EvaluationStack.Push(transaction.Hash.ToArray());

            return true;
        }

        protected virtual bool Transaction_GetType(IExecutionEngine engine)
        {
            var transaction = engine.CurrentContext.EvaluationStack.PopObject<Transaction>();
            if (transaction == null) return false;

            engine.CurrentContext.EvaluationStack.Push((int)transaction.Type);

            return true;
        }

        protected virtual bool Transaction_GetAttributes(IExecutionEngine engine)
        {
            var transaction = engine.CurrentContext.EvaluationStack.PopObject<Transaction>();
            if (transaction == null) return false;
            if (transaction.Attributes.Length > EngineMaxArraySize)
                return false;

            engine.CurrentContext.EvaluationStack.PushArray(transaction.Attributes);

            return true;
        }

        protected virtual bool Transaction_GetInputs(IExecutionEngine engine)
        {
            var transaction = engine.CurrentContext.EvaluationStack.PopObject<Transaction>();
            if (transaction == null) return false;
            if (transaction.Inputs.Length > EngineMaxArraySize)
                return false;

            engine.CurrentContext.EvaluationStack.PushArray(transaction.Inputs);

            return true;
        }

        protected virtual bool Transaction_GetOutputs(IExecutionEngine engine)
        {
            var transaction = engine.CurrentContext.EvaluationStack.PopObject<Transaction>();
            if (transaction == null) return false;
            if (transaction.Outputs.Length > EngineMaxArraySize)
                return false;

            engine.CurrentContext.EvaluationStack.PushArray(transaction.Outputs);

            return true;
        }

        protected virtual bool Transaction_GetReferences(IExecutionEngine engine)
        {
            var transaction = engine.CurrentContext.EvaluationStack.PopObject<Transaction>();
            if (transaction == null) return false;
            // TODO: Check refs length?
            if (transaction.Inputs.Length > EngineMaxArraySize)
                return false;

            var references = _transactionRepository.GetReferences(transaction).Result;
            engine.CurrentContext.EvaluationStack.PushArray(transaction.Inputs.Select(i => references[i]).ToArray());

            return true;
        }

        protected virtual bool Transaction_GetUnspentCoins(IExecutionEngine engine)
        {
            var transaction = engine.CurrentContext.EvaluationStack.PopObject<Transaction>();
            if (transaction == null) return false;

            var outputs = _transactionRepository.GetUnspent(transaction.Hash).Result;
            if (outputs.Count() > EngineMaxArraySize)
                return false;

            engine.CurrentContext.EvaluationStack.PushArray(outputs.ToArray());

            return true;
        }

        protected virtual bool InvocationTransaction_GetScript(IExecutionEngine engine)
        {
            var transaction = engine.CurrentContext.EvaluationStack.PopObject<InvocationTransaction>();
            if (transaction == null) return false;

            engine.CurrentContext.EvaluationStack.Push(transaction.Script);

            return true;
        }

        protected virtual bool Attribute_GetUsage(IExecutionEngine engine)
        {
            var transactionAttr = engine.CurrentContext.EvaluationStack.PopObject<TransactionAttribute>();
            if (transactionAttr == null) return false;

            engine.CurrentContext.EvaluationStack.Push((int)transactionAttr.Usage);

            return true;
        }

        protected virtual bool Attribute_GetData(IExecutionEngine engine)
        {
            var transactionAttr = engine.CurrentContext.EvaluationStack.PopObject<TransactionAttribute>();
            if (transactionAttr == null) return false;

            engine.CurrentContext.EvaluationStack.Push(transactionAttr.Data);

            return true;
        }

        protected virtual bool Input_GetHash(IExecutionEngine engine)
        {
            var coinReference = engine.CurrentContext.EvaluationStack.PopObject<CoinReference>();
            if (coinReference == null) return false;

            engine.CurrentContext.EvaluationStack.Push(coinReference.PrevHash.ToArray());

            return true;
        }

        protected virtual bool Input_GetIndex(IExecutionEngine engine)
        {
            var coinReference = engine.CurrentContext.EvaluationStack.PopObject<CoinReference>();
            if (coinReference == null) return false;

            engine.CurrentContext.EvaluationStack.Push((int)coinReference.PrevIndex);

            return true;
        }

        protected virtual bool Output_GetAssetId(IExecutionEngine engine)
        {
            var transactionOutput = engine.CurrentContext.EvaluationStack.PopObject<TransactionOutput>();
            if (transactionOutput == null) return false;

            engine.CurrentContext.EvaluationStack.Push(transactionOutput.AssetId.ToArray());

            return true;
        }

        protected virtual bool Output_GetValue(IExecutionEngine engine)
        {
            var transactionOutput = engine.CurrentContext.EvaluationStack.PopObject<TransactionOutput>();
            if (transactionOutput == null) return false;

            engine.CurrentContext.EvaluationStack.Push(transactionOutput.Value.Value);

            return true;
        }

        protected virtual bool Output_GetScriptHash(IExecutionEngine engine)
        {
            var transactionOutput = engine.CurrentContext.EvaluationStack.PopObject<TransactionOutput>();
            if (transactionOutput == null) return false;

            engine.CurrentContext.EvaluationStack.Push(transactionOutput.ScriptHash.ToArray());

            return true;
        }

        protected virtual bool Account_GetScriptHash(IExecutionEngine engine)
        {
            var account = engine.CurrentContext.EvaluationStack.PopObject<Account>();
            if (account == null) return false;

            engine.CurrentContext.EvaluationStack.Push(account.ScriptHash.ToArray());

            return true;
        }

        protected virtual bool Account_GetVotes(IExecutionEngine engine)
        {
            var account = engine.CurrentContext.EvaluationStack.PopObject<Account>();
            if (account == null) return false;

            // TODO: it was EncodePoint before. Check with NEO
            account.Votes.Select(v => v.EncodedData).ForEach(engine.CurrentContext.EvaluationStack.Push);

            return true;
        }

        protected virtual bool Account_GetBalance(IExecutionEngine engine)
        {
            var account = engine.CurrentContext.EvaluationStack.PopObject<Account>();
            if (account == null) return false;

            var assetId = new UInt256(engine.CurrentContext.EvaluationStack.PopByteArray());
            var balance = account.Balances.TryGetValue(assetId, out var value) ? value : Fixed8.Zero;

            engine.CurrentContext.EvaluationStack.Push(balance.Value);

            return true;
        }

        protected virtual bool Asset_GetAssetId(IExecutionEngine engine)
        {
            var asset = engine.CurrentContext.EvaluationStack.PopObject<Asset>();
            if (asset == null) return false;

            engine.CurrentContext.EvaluationStack.Push(asset.Id.ToArray());

            return true;
        }

        protected virtual bool Asset_GetAssetType(IExecutionEngine engine)
        {
            var asset = engine.CurrentContext.EvaluationStack.PopObject<Asset>();
            if (asset == null) return false;

            engine.CurrentContext.EvaluationStack.Push((int)asset.AssetType);

            return true;
        }

        protected virtual bool Asset_GetAmount(IExecutionEngine engine)
        {
            var asset = engine.CurrentContext.EvaluationStack.PopObject<Asset>();
            if (asset == null) return false;

            engine.CurrentContext.EvaluationStack.Push(asset.Amount.Value);

            return true;
        }

        protected virtual bool Asset_GetAvailable(IExecutionEngine engine)
        {
            var asset = engine.CurrentContext.EvaluationStack.PopObject<Asset>();
            if (asset == null) return false;

            engine.CurrentContext.EvaluationStack.Push(asset.Available.Value);

            return true;
        }

        protected virtual bool Asset_GetPrecision(IExecutionEngine engine)
        {
            var asset = engine.CurrentContext.EvaluationStack.PopObject<Asset>();
            if (asset == null) return false;

            engine.CurrentContext.EvaluationStack.Push((int)asset.Precision);

            return true;
        }

        protected virtual bool Asset_GetOwner(IExecutionEngine engine)
        {
            var asset = engine.CurrentContext.EvaluationStack.PopObject<Asset>();
            if (asset == null) return false;

            // TODO: it was EncodePoint before. Check with NEO
            engine.CurrentContext.EvaluationStack.Push(asset.Owner.EncodedData);

            return true;
        }

        protected virtual bool Asset_GetAdmin(IExecutionEngine engine)
        {
            var asset = engine.CurrentContext.EvaluationStack.PopObject<Asset>();
            if (asset == null) return false;

            engine.CurrentContext.EvaluationStack.Push(asset.Admin.ToArray());

            return true;
        }

        protected virtual bool Asset_GetIssuer(IExecutionEngine engine)
        {
            var asset = engine.CurrentContext.EvaluationStack.PopObject<Asset>();
            if (asset == null) return false;

            engine.CurrentContext.EvaluationStack.Push(asset.Issuer.ToArray());

            return true;
        }

        protected virtual bool Contract_GetScript(IExecutionEngine engine)
        {
            var contract = engine.CurrentContext.EvaluationStack.PopObject<Contract>();
            if (contract == null) return false;

            engine.CurrentContext.EvaluationStack.Push(contract.Script);

            return true;
        }

        protected virtual bool Contract_IsPayable(IExecutionEngine engine)
        {
            var contract = engine.CurrentContext.EvaluationStack.PopObject<Contract>();
            if (contract == null) return false;

            engine.CurrentContext.EvaluationStack.Push(contract.Payable);

            return true;
        }

        protected virtual bool Storage_GetContext(IExecutionEngine engine)
        {
            engine.CurrentContext.EvaluationStack.PushObject(new StorageContext
            (
                new UInt160(engine.CurrentContext.ScriptHash),
                false
            ));

            return true;
        }

        protected virtual bool Storage_GetReadOnlyContext(IExecutionEngine engine)
        {
            engine.CurrentContext.EvaluationStack.PushObject(new StorageContext
            (
                new UInt160(engine.CurrentContext.ScriptHash),
                true
            ));

            return true;
        }

        protected virtual bool Storage_Get(IExecutionEngine engine)
        {
            var storageContext = engine.CurrentContext.EvaluationStack.PopObject<StorageContext>();
            if (storageContext == null) return false;
            if (!CheckStorageContext(storageContext)) return false;

            var key = engine.CurrentContext.EvaluationStack.PopByteArray();
            var item = Storages.TryGet(new StorageKey
            {
                ScriptHash = storageContext.ScriptHash,
                Key = key
            });

            engine.CurrentContext.EvaluationStack.Push(item?.Value ?? Array.Empty<byte>());

            return true;
        }

        protected virtual bool Storage_Find(IExecutionEngine engine)
        {
            var storageContext = engine.CurrentContext.EvaluationStack.PopObject<StorageContext>();
            if (storageContext == null) return false;
            if (!CheckStorageContext(storageContext)) return false;

            var prefix = engine.CurrentContext.EvaluationStack.PopByteArray();
            byte[] prefixKey;

            using (var ms = new MemoryStream())
            {
                var index = 0;
                var remain = prefix.Length;

                while (remain >= 16)
                {
                    ms.Write(prefix, index, 16);
                    ms.WriteByte(0);
                    index += 16;
                    remain -= 16;
                }

                if (remain > 0)
                    ms.Write(prefix, index, remain);

                prefixKey = storageContext.ScriptHash.ToArray().Concat(ms.ToArray()).ToArray();
            }

            // TODO: Find a better way not to expose engine.CurrentContext.EvaluationStackItem types or Create* methods
            var enumerator = Storages.Find(prefixKey)
                .Where(p => p.Key.Key.Take(prefix.Length).SequenceEqual(prefix))
                .Select(p => new KeyValuePair<StackItemBase, StackItemBase>(engine.CurrentContext.EvaluationStack.CreateByteArray(p.Key.Key), engine.CurrentContext.EvaluationStack.CreateByteArray(p.Value.Value)))
                .GetEnumerator();
            var keyEnumerator = new KeyEnumerator(enumerator);

            engine.CurrentContext.EvaluationStack.PushObject(keyEnumerator);
            _disposables.Add(keyEnumerator);

            return true;
        }

        protected virtual bool StorageContext_AsReadOnly(IExecutionEngine engine)
        {
            var storageContext = engine.CurrentContext.EvaluationStack.PopObject<StorageContext>();
            if (storageContext == null) return false;
            if (!storageContext.IsReadOnly)
                storageContext = new StorageContext
                (
                    storageContext.ScriptHash,
                    true
                );

            engine.CurrentContext.EvaluationStack.PushObject(storageContext);

            return true;
        }
    }
}