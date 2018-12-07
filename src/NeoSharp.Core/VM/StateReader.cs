using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
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
        private readonly InteropService _interopService;

        private readonly ETriggerType _trigger;
        //public static event EventHandler<NotifyEventArgs> Notify;
        //public static event EventHandler<LogEventArgs> Log;

        //private readonly List<NotifyEventArgs> _notifications = new List<NotifyEventArgs>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        //public IReadOnlyList<NotifyEventArgs> Notifications => _notifications;

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
            ETriggerType trigger)
        {
            Accounts = accounts;
            Assets = assets;
            Contracts = contracts;
            Storages = storages;
            _blockchainContext = blockchainContext;
            _blockRepository = blockRepository;
            _transactionRepository = transactionRepository;
            _trigger = trigger;
            _interopService = interopService;

            //Standard Library
            interopService.RegisterStackTransition("System.Runtime.GetTrigger", Runtime_GetTrigger);
            interopService.RegisterStackTransition("System.Runtime.CheckWitness", Runtime_CheckWitness);
            interopService.RegisterStackTransition("System.Runtime.Notify", Runtime_Notify);
            interopService.RegisterStackTransition("System.Runtime.Log", Runtime_Log);
            interopService.RegisterStackTransition("System.Runtime.GetTime", Runtime_GetTime);
            interopService.RegisterStackTransition("System.Blockchain.GetHeight", Blockchain_GetHeight);
            interopService.RegisterStackTransition("System.Blockchain.GetHeader", Blockchain_GetHeader);
            interopService.RegisterStackTransition("System.Blockchain.GetBlock", Blockchain_GetBlock);
            interopService.RegisterStackTransition("System.Blockchain.GetTransaction", Blockchain_GetTransaction);
            interopService.RegisterStackTransition("System.Blockchain.GetTransactionHeight", Blockchain_GetTransactionHeight);
            interopService.RegisterStackTransition("System.Blockchain.GetContract", Blockchain_GetContract);
            interopService.RegisterStackTransition("System.Header.GetIndex", Header_GetIndex);
            interopService.RegisterStackTransition("System.Header.GetHash", Header_GetHash);
            interopService.RegisterStackTransition("System.Header.GetPrevHash", Header_GetPrevHash);
            interopService.RegisterStackTransition("System.Header.GetTimestamp", Header_GetTimestamp);
            interopService.RegisterStackTransition("System.Block.GetTransactionCount", Block_GetTransactionCount);
            interopService.RegisterStackTransition("System.Block.GetTransactions", Block_GetTransactions);
            interopService.RegisterStackTransition("System.Block.GetTransaction", Block_GetTransaction);
            interopService.RegisterStackTransition("System.Transaction.GetHash", Transaction_GetHash);
            interopService.RegisterStackTransition("System.Storage.GetContext", Storage_GetContext);
            interopService.RegisterStackTransition("System.Storage.GetReadOnlyContext", Storage_GetReadOnlyContext);
            interopService.RegisterStackTransition("System.Storage.Get", Storage_Get);
            interopService.RegisterStackTransition("System.StorageContext.AsReadOnly", StorageContext_AsReadOnly);

            //Neo Specified
            interopService.RegisterStackTransition("Neo.Blockchain.GetAccount", Blockchain_GetAccount);
            interopService.RegisterStackTransition("Neo.Blockchain.GetValidators", Blockchain_GetValidators);
            interopService.RegisterStackTransition("Neo.Blockchain.GetAsset", Blockchain_GetAsset);
            interopService.RegisterStackTransition("Neo.Header.GetVersion", Header_GetVersion);
            interopService.RegisterStackTransition("Neo.Header.GetMerkleRoot", Header_GetMerkleRoot);
            interopService.RegisterStackTransition("Neo.Header.GetConsensusData", Header_GetConsensusData);
            interopService.RegisterStackTransition("Neo.Header.GetNextConsensus", Header_GetNextConsensus);
            interopService.RegisterStackTransition("Neo.Transaction.GetType", Transaction_GetType);
            interopService.RegisterStackTransition("Neo.Transaction.GetAttributes", Transaction_GetAttributes);
            interopService.RegisterStackTransition("Neo.Transaction.GetInputs", Transaction_GetInputs);
            interopService.RegisterStackTransition("Neo.Transaction.GetOutputs", Transaction_GetOutputs);
            interopService.RegisterStackTransition("Neo.Transaction.GetReferences", Transaction_GetReferences);
            interopService.RegisterStackTransition("Neo.Transaction.GetUnspentCoins", Transaction_GetUnspentCoins);
            interopService.RegisterStackTransition("Neo.InvocationTransaction.GetScript", InvocationTransaction_GetScript);
            interopService.RegisterStackTransition("Neo.Attribute.GetUsage", Attribute_GetUsage);
            interopService.RegisterStackTransition("Neo.Attribute.GetData", Attribute_GetData);
            interopService.RegisterStackTransition("Neo.Input.GetHash", Input_GetHash);
            interopService.RegisterStackTransition("Neo.Input.GetIndex", Input_GetIndex);
            interopService.RegisterStackTransition("Neo.Output.GetAssetId", Output_GetAssetId);
            interopService.RegisterStackTransition("Neo.Output.GetValue", Output_GetValue);
            interopService.RegisterStackTransition("Neo.Output.GetScriptHash", Output_GetScriptHash);
            interopService.RegisterStackTransition("Neo.Account.GetScriptHash", Account_GetScriptHash);
            interopService.RegisterStackTransition("Neo.Account.GetVotes", Account_GetVotes);
            interopService.RegisterStackTransition("Neo.Account.GetBalance", Account_GetBalance);
            interopService.RegisterStackTransition("Neo.Asset.GetAssetId", Asset_GetAssetId);
            interopService.RegisterStackTransition("Neo.Asset.GetAssetType", Asset_GetAssetType);
            interopService.RegisterStackTransition("Neo.Asset.GetAmount", Asset_GetAmount);
            interopService.RegisterStackTransition("Neo.Asset.GetAvailable", Asset_GetAvailable);
            interopService.RegisterStackTransition("Neo.Asset.GetPrecision", Asset_GetPrecision);
            interopService.RegisterStackTransition("Neo.Asset.GetOwner", Asset_GetOwner);
            interopService.RegisterStackTransition("Neo.Asset.GetAdmin", Asset_GetAdmin);
            interopService.RegisterStackTransition("Neo.Asset.GetIssuer", Asset_GetIssuer);
            interopService.RegisterStackTransition("Neo.Contract.GetScript", Contract_GetScript);
            interopService.RegisterStackTransition("Neo.Contract.IsPayable", Contract_IsPayable);
            interopService.RegisterStackTransition("Neo.Storage.Find", Storage_Find);
            // TODO: APIs for enumeration and iteration
            //interopService.RegisterStackTransition("Neo.Enumerator.Create", Enumerator_Create);
            //interopService.RegisterStackTransition("Neo.Enumerator.Next", Enumerator_Next);
            //interopService.RegisterStackTransition("Neo.Enumerator.Value", Enumerator_Value);
            //interopService.RegisterStackTransition("Neo.Enumerator.Concat", Enumerator_Concat);
            //interopService.RegisterStackTransition("Neo.Iterator.Create", Iterator_Create);
            //interopService.RegisterStackTransition("Neo.Iterator.Key", Iterator_Key);
            //interopService.RegisterStackTransition("Neo.Iterator.Keys", Iterator_Keys);
            //interopService.RegisterStackTransition("Neo.Iterator.Values", Iterator_Values);

            #region Aliases
            //interopService.RegisterStackTransition("Neo.Iterator.Next", Enumerator_Next);
            //interopService.RegisterStackTransition("Neo.Iterator.Value", Enumerator_Value);
            #endregion

            #region Old APIs
            interopService.RegisterStackTransition("Neo.Runtime.GetTrigger", Runtime_GetTrigger);
            interopService.RegisterStackTransition("Neo.Runtime.CheckWitness", Runtime_CheckWitness);
            interopService.RegisterStackTransition("AntShares.Runtime.CheckWitness", Runtime_CheckWitness);
            interopService.RegisterStackTransition("Neo.Runtime.Notify", Runtime_Notify);
            interopService.RegisterStackTransition("AntShares.Runtime.Notify", Runtime_Notify);
            interopService.RegisterStackTransition("Neo.Runtime.Log", Runtime_Log);
            interopService.RegisterStackTransition("AntShares.Runtime.Log", Runtime_Log);
            interopService.RegisterStackTransition("Neo.Runtime.GetTime", Runtime_GetTime);
            interopService.RegisterStackTransition("Neo.Blockchain.GetHeight", Blockchain_GetHeight);
            interopService.RegisterStackTransition("AntShares.Blockchain.GetHeight", Blockchain_GetHeight);
            interopService.RegisterStackTransition("Neo.Blockchain.GetHeader", Blockchain_GetHeader);
            interopService.RegisterStackTransition("AntShares.Blockchain.GetHeader", Blockchain_GetHeader);
            interopService.RegisterStackTransition("Neo.Blockchain.GetBlock", Blockchain_GetBlock);
            interopService.RegisterStackTransition("AntShares.Blockchain.GetBlock", Blockchain_GetBlock);
            interopService.RegisterStackTransition("Neo.Blockchain.GetTransaction", Blockchain_GetTransaction);
            interopService.RegisterStackTransition("AntShares.Blockchain.GetTransaction", Blockchain_GetTransaction);
            interopService.RegisterStackTransition("Neo.Blockchain.GetTransactionHeight", Blockchain_GetTransactionHeight);
            interopService.RegisterStackTransition("AntShares.Blockchain.GetAccount", Blockchain_GetAccount);
            interopService.RegisterStackTransition("AntShares.Blockchain.GetValidators", Blockchain_GetValidators);
            interopService.RegisterStackTransition("AntShares.Blockchain.GetAsset", Blockchain_GetAsset);
            interopService.RegisterStackTransition("Neo.Blockchain.GetContract", Blockchain_GetContract);
            interopService.RegisterStackTransition("AntShares.Blockchain.GetContract", Blockchain_GetContract);
            interopService.RegisterStackTransition("Neo.Header.GetIndex", Header_GetIndex);
            interopService.RegisterStackTransition("Neo.Header.GetHash", Header_GetHash);
            interopService.RegisterStackTransition("AntShares.Header.GetHash", Header_GetHash);
            interopService.RegisterStackTransition("AntShares.Header.GetVersion", Header_GetVersion);
            interopService.RegisterStackTransition("Neo.Header.GetPrevHash", Header_GetPrevHash);
            interopService.RegisterStackTransition("AntShares.Header.GetPrevHash", Header_GetPrevHash);
            interopService.RegisterStackTransition("AntShares.Header.GetMerkleRoot", Header_GetMerkleRoot);
            interopService.RegisterStackTransition("Neo.Header.GetTimestamp", Header_GetTimestamp);
            interopService.RegisterStackTransition("AntShares.Header.GetTimestamp", Header_GetTimestamp);
            interopService.RegisterStackTransition("AntShares.Header.GetConsensusData", Header_GetConsensusData);
            interopService.RegisterStackTransition("AntShares.Header.GetNextConsensus", Header_GetNextConsensus);
            interopService.RegisterStackTransition("Neo.Block.GetTransactionCount", Block_GetTransactionCount);
            interopService.RegisterStackTransition("AntShares.Block.GetTransactionCount", Block_GetTransactionCount);
            interopService.RegisterStackTransition("Neo.Block.GetTransactions", Block_GetTransactions);
            interopService.RegisterStackTransition("AntShares.Block.GetTransactions", Block_GetTransactions);
            interopService.RegisterStackTransition("Neo.Block.GetTransaction", Block_GetTransaction);
            interopService.RegisterStackTransition("AntShares.Block.GetTransaction", Block_GetTransaction);
            interopService.RegisterStackTransition("Neo.Transaction.GetHash", Transaction_GetHash);
            interopService.RegisterStackTransition("AntShares.Transaction.GetHash", Transaction_GetHash);
            interopService.RegisterStackTransition("AntShares.Transaction.GetType", Transaction_GetType);
            interopService.RegisterStackTransition("AntShares.Transaction.GetAttributes", Transaction_GetAttributes);
            interopService.RegisterStackTransition("AntShares.Transaction.GetInputs", Transaction_GetInputs);
            interopService.RegisterStackTransition("AntShares.Transaction.GetOutputs", Transaction_GetOutputs);
            interopService.RegisterStackTransition("AntShares.Transaction.GetReferences", Transaction_GetReferences);
            interopService.RegisterStackTransition("AntShares.Attribute.GetUsage", Attribute_GetUsage);
            interopService.RegisterStackTransition("AntShares.Attribute.GetData", Attribute_GetData);
            interopService.RegisterStackTransition("AntShares.Input.GetHash", Input_GetHash);
            interopService.RegisterStackTransition("AntShares.Input.GetIndex", Input_GetIndex);
            interopService.RegisterStackTransition("AntShares.Output.GetAssetId", Output_GetAssetId);
            interopService.RegisterStackTransition("AntShares.Output.GetValue", Output_GetValue);
            interopService.RegisterStackTransition("AntShares.Output.GetScriptHash", Output_GetScriptHash);
            interopService.RegisterStackTransition("AntShares.Account.GetScriptHash", Account_GetScriptHash);
            interopService.RegisterStackTransition("AntShares.Account.GetVotes", Account_GetVotes);
            interopService.RegisterStackTransition("AntShares.Account.GetBalance", Account_GetBalance);
            interopService.RegisterStackTransition("AntShares.Asset.GetAssetId", Asset_GetAssetId);
            interopService.RegisterStackTransition("AntShares.Asset.GetAssetType", Asset_GetAssetType);
            interopService.RegisterStackTransition("AntShares.Asset.GetAmount", Asset_GetAmount);
            interopService.RegisterStackTransition("AntShares.Asset.GetAvailable", Asset_GetAvailable);
            interopService.RegisterStackTransition("AntShares.Asset.GetPrecision", Asset_GetPrecision);
            interopService.RegisterStackTransition("AntShares.Asset.GetOwner", Asset_GetOwner);
            interopService.RegisterStackTransition("AntShares.Asset.GetAdmin", Asset_GetAdmin);
            interopService.RegisterStackTransition("AntShares.Asset.GetIssuer", Asset_GetIssuer);
            interopService.RegisterStackTransition("AntShares.Contract.GetScript", Contract_GetScript);
            interopService.RegisterStackTransition("Neo.Storage.GetContext", Storage_GetContext);
            interopService.RegisterStackTransition("AntShares.Storage.GetContext", Storage_GetContext);
            interopService.RegisterStackTransition("Neo.Storage.GetReadOnlyContext", Storage_GetReadOnlyContext);
            interopService.RegisterStackTransition("Neo.Storage.Get", Storage_Get);
            interopService.RegisterStackTransition("AntShares.Storage.Get", Storage_Get);
            interopService.RegisterStackTransition("Neo.StorageContext.AsReadOnly", StorageContext_AsReadOnly);
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

        protected virtual bool Runtime_GetTrigger(IStackAccessor stack)
        {
            stack.Push((int)_trigger);
            return true;
        }

        protected bool CheckWitness(ExecutionEngineBase engine, UInt160 hash)
        {
            // TODO:
            //IVerifiable container = (IVerifiable)engine.MessageProvider;
            //UInt160[] _hashes_for_verifying = container.GetScriptHashesForVerifying();
            //return _hashes_for_verifying.Contains(hash);
            return true;
        }

        protected bool CheckWitness(ExecutionEngineBase engine, ECPoint pubkey)
        {
            // TODO:
            //return CheckWitness(engine, Contract.CreateSignatureRedeemScript(pubkey).ToScriptHash());
            return true;
        }

        protected virtual bool Runtime_CheckWitness(IStackAccessor stack)
        {
            var hashOrPubkey = stack.PopByteArray();
            // TODO:
            //bool result;
            //if (hashOrPubkey.Length == 20)
            //    result = CheckWitness(engine, new UInt160(hashOrPubkey));
            //else if (hashOrPubkey.Length == 33)
            //    result = CheckWitness(engine, new ECPoint(hashOrPubkey));
            //else
            //    return false;
            //stack.Push(result);
            return true;
        }

        protected virtual bool Runtime_Notify(IStackAccessor stack)
        {
            var state = stack.Pop<StackItemBase>();

            _interopService.RaiseOnNotify(new NotifyEventArgs(stack.ScriptHash.ToArray(), state));

            return true;
        }

        protected virtual bool Runtime_Log(IStackAccessor stack)
        {
            var message = Encoding.UTF8.GetString(stack.PopByteArray());

            _interopService.RaiseOnLog(new LogEventArgs(stack.ScriptHash.ToArray(), message));

            return true;
        }

        protected virtual bool Runtime_GetTime(IStackAccessor stack)
        {
            stack.Push(_blockchainContext.LastBlockHeader.Timestamp + 15);
            return true;
        }

        protected virtual bool Blockchain_GetHeight(IStackAccessor stack)
        {
            stack.Push(_blockchainContext.CurrentBlock.Index);
            return true;
        }

        protected virtual bool Blockchain_GetHeader(IStackAccessor stack)
        {
            var data = stack.PopByteArray();

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

            stack.Push(blockHeader);

            return true;
        }

        protected virtual bool Blockchain_GetBlock(IStackAccessor stack)
        {
            var data = stack.PopByteArray();

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

            stack.Push(block);

            return true;
        }

        protected virtual bool Blockchain_GetTransaction(IStackAccessor stack)
        {
            var hash = stack.PopByteArray();
            var transaction = _transactionRepository.GetTransaction(new UInt256(hash)).Result;

            stack.Push(transaction);

            return true;
        }

        protected virtual bool Blockchain_GetTransactionHeight(IStackAccessor stack)
        {
            var hash = stack.PopByteArray();
            // TODO: looks like we need an index transaction in the block;
            var height = 0;
            // var transaction = _transactionRepository.GetTransaction(new UInt256(hash)).Result;

            stack.Push(height);

            return true;
        }

        protected virtual bool Blockchain_GetAccount(IStackAccessor stack)
        {
            var hash = new UInt160(stack.PopByteArray());
            var account = Accounts.GetOrAdd(hash, () => new Account(hash));
            stack.Push(account);
            return true;
        }

        protected virtual bool Blockchain_GetValidators(IStackAccessor stack)
        {
            // TODO: looks like we need to get all validators
            //ECPoint[] validators = _blockchain.GetValidators();
            //stack.Push(validators.Select(p => (StackItem)p.EncodePoint(true)).ToArray());
            return true;
        }

        protected virtual bool Blockchain_GetAsset(IStackAccessor stack)
        {
            var hash = new UInt256(stack.PopByteArray());
            var asset = Assets.TryGet(hash);
            if (asset == null) return false;
            stack.Push(asset);
            return true;
        }

        protected virtual bool Blockchain_GetContract(IStackAccessor stack)
        {
            var hash = new UInt160(stack.PopByteArray());
            var contract = Contracts.TryGet(hash);
            if (contract == null)
                stack.Push(Array.Empty<byte>());
            else
                stack.Push(contract);
            return true;
        }

        protected virtual bool Header_GetIndex(IStackAccessor stack)
        {
            var blockBase = stack.Pop<BlockBase>();
            if (blockBase == null) return false;

            stack.Push(blockBase.Index);

            return true;
        }

        protected virtual bool Header_GetHash(IStackAccessor stack)
        {
            var blockBase = stack.Pop<BlockBase>();
            if (blockBase == null) return false;

            stack.Push(blockBase.Hash.ToArray());

            return true;
        }

        protected virtual bool Header_GetVersion(IStackAccessor stack)
        {
            var blockBase = stack.Pop<BlockBase>();
            if (blockBase == null) return false;

            stack.Push(blockBase.Version);

            return true;
        }

        protected virtual bool Header_GetPrevHash(IStackAccessor stack)
        {
            var blockBase = stack.Pop<BlockBase>();
            if (blockBase == null) return false;

            stack.Push(blockBase.PreviousBlockHash.ToArray());

            return true;
        }

        protected virtual bool Header_GetMerkleRoot(IStackAccessor stack)
        {
            // TODO: Should MerkleRoot be in BlockBase?
            var blockBase = stack.Pop<BlockHeader>();
            if (blockBase == null) return false;

            stack.Push(blockBase.MerkleRoot.ToArray());

            return true;
        }

        protected virtual bool Header_GetTimestamp(IStackAccessor stack)
        {
            var blockBase = stack.Pop<BlockBase>();
            if (blockBase == null) return false;

            stack.Push(blockBase.Timestamp);

            return true;
        }

        protected virtual bool Header_GetConsensusData(IStackAccessor stack)
        {
            var blockBase = stack.Pop<BlockBase>();
            if (blockBase == null) return false;

            stack.Push(blockBase.ConsensusData);

            return true;
        }

        protected virtual bool Header_GetNextConsensus(IStackAccessor stack)
        {
            var blockBase = stack.Pop<BlockBase>();
            if (blockBase == null) return false;

            stack.Push(blockBase.NextConsensus.ToArray());

            return true;
        }

        protected virtual bool Block_GetTransactionCount(IStackAccessor stack)
        {
            var block = stack.Pop<Block>();
            if (block == null) return false;

            stack.Push(block.Transactions.Length);

            return true;
        }

        protected virtual bool Block_GetTransactions(IStackAccessor stack)
        {
            var block = stack.Pop<Block>();
            if (block == null) return false;
            if (block.Transactions.Length > EngineMaxArraySize) return false;

            stack.Push(block.Transactions);

            return true;
        }

        protected virtual bool Block_GetTransaction(IStackAccessor stack)
        {
            var block = stack.Pop<Block>();
            if (block == null) return false;

            var index = (int)stack.PopBigInteger();
            if (index < 0 || index >= block.Transactions.Length) return false;

            var transaction = block.Transactions[index];

            stack.Push(transaction);

            return true;
        }

        protected virtual bool Transaction_GetHash(IStackAccessor stack)
        {
            var transaction = stack.Pop<Transaction>();
            if (transaction == null) return false;

            stack.Push(transaction.Hash.ToArray());

            return true;
        }

        protected virtual bool Transaction_GetType(IStackAccessor stack)
        {
            var transaction = stack.Pop<Transaction>();
            if (transaction == null) return false;

            stack.Push((int)transaction.Type);

            return true;
        }

        protected virtual bool Transaction_GetAttributes(IStackAccessor stack)
        {
            var transaction = stack.Pop<Transaction>();
            if (transaction == null) return false;
            if (transaction.Attributes.Length > EngineMaxArraySize)
                return false;

            stack.Push(transaction.Attributes);

            return true;
        }

        protected virtual bool Transaction_GetInputs(IStackAccessor stack)
        {
            var transaction = stack.Pop<Transaction>();
            if (transaction == null) return false;
            if (transaction.Inputs.Length > EngineMaxArraySize)
                return false;

            stack.Push(transaction.Inputs);

            return true;
        }

        protected virtual bool Transaction_GetOutputs(IStackAccessor stack)
        {
            var transaction = stack.Pop<Transaction>();
            if (transaction == null) return false;
            if (transaction.Outputs.Length > EngineMaxArraySize)
                return false;

            stack.Push(transaction.Outputs);

            return true;
        }

        protected virtual bool Transaction_GetReferences(IStackAccessor stack)
        {
            var transaction = stack.Pop<Transaction>();
            if (transaction == null) return false;
            // TODO: Check refs length?
            if (transaction.Inputs.Length > EngineMaxArraySize)
                return false;

            var references = _transactionRepository.GetReferences(transaction).Result;
            stack.Push(transaction.Inputs.Select(i => references[i]).ToArray());

            return true;
        }

        protected virtual bool Transaction_GetUnspentCoins(IStackAccessor stack)
        {
            var transaction = stack.Pop<Transaction>();
            if (transaction == null) return false;

            var outputs = _transactionRepository.GetUnspent(transaction.Hash).Result;
            if (outputs.Count() > EngineMaxArraySize)
                return false;

            stack.Push(outputs.ToArray());

            return true;
        }

        protected virtual bool InvocationTransaction_GetScript(IStackAccessor stack)
        {
            var transaction = stack.Pop<InvocationTransaction>();
            if (transaction == null) return false;

            stack.Push(transaction.Script);

            return true;
        }

        protected virtual bool Attribute_GetUsage(IStackAccessor stack)
        {
            var transactionAttr = stack.Pop<TransactionAttribute>();
            if (transactionAttr == null) return false;

            stack.Push((int)transactionAttr.Usage);

            return true;
        }

        protected virtual bool Attribute_GetData(IStackAccessor stack)
        {
            var transactionAttr = stack.Pop<TransactionAttribute>();
            if (transactionAttr == null) return false;

            stack.Push(transactionAttr.Data);

            return true;
        }

        protected virtual bool Input_GetHash(IStackAccessor stack)
        {
            var coinReference = stack.Pop<CoinReference>();
            if (coinReference == null) return false;

            stack.Push(coinReference.PrevHash.ToArray());

            return true;
        }

        protected virtual bool Input_GetIndex(IStackAccessor stack)
        {
            var coinReference = stack.Pop<CoinReference>();
            if (coinReference == null) return false;

            stack.Push((int)coinReference.PrevIndex);

            return true;
        }

        protected virtual bool Output_GetAssetId(IStackAccessor stack)
        {
            var transactionOutput = stack.Pop<TransactionOutput>();
            if (transactionOutput == null) return false;

            stack.Push(transactionOutput.AssetId.ToArray());

            return true;
        }

        protected virtual bool Output_GetValue(IStackAccessor stack)
        {
            var transactionOutput = stack.Pop<TransactionOutput>();
            if (transactionOutput == null) return false;

            stack.Push(transactionOutput.Value.Value);

            return true;
        }

        protected virtual bool Output_GetScriptHash(IStackAccessor stack)
        {
            var transactionOutput = stack.Pop<TransactionOutput>();
            if (transactionOutput == null) return false;

            stack.Push(transactionOutput.ScriptHash.ToArray());

            return true;
        }

        protected virtual bool Account_GetScriptHash(IStackAccessor stack)
        {
            var account = stack.Pop<Account>();
            if (account == null) return false;

            stack.Push(account.ScriptHash.ToArray());

            return true;
        }

        protected virtual bool Account_GetVotes(IStackAccessor stack)
        {
            var account = stack.Pop<Account>();
            if (account == null) return false;

            // TODO: it was EncodePoint before. Check with NEO
            account.Votes.Select(v => v.EncodedData).ForEach(stack.Push);

            return true;
        }

        protected virtual bool Account_GetBalance(IStackAccessor stack)
        {
            var account = stack.Pop<Account>();
            if (account == null) return false;

            var assetId = new UInt256(stack.PopByteArray());
            var balance = account.Balances.TryGetValue(assetId, out var value) ? value : Fixed8.Zero;

            stack.Push(balance.Value);

            return true;
        }

        protected virtual bool Asset_GetAssetId(IStackAccessor stack)
        {
            var asset = stack.Pop<Asset>();
            if (asset == null) return false;

            stack.Push(asset.Id.ToArray());

            return true;
        }

        protected virtual bool Asset_GetAssetType(IStackAccessor stack)
        {
            var asset = stack.Pop<Asset>();
            if (asset == null) return false;

            stack.Push((int)asset.AssetType);

            return true;
        }

        protected virtual bool Asset_GetAmount(IStackAccessor stack)
        {
            var asset = stack.Pop<Asset>();
            if (asset == null) return false;

            stack.Push(asset.Amount.Value);

            return true;
        }

        protected virtual bool Asset_GetAvailable(IStackAccessor stack)
        {
            var asset = stack.Pop<Asset>();
            if (asset == null) return false;

            stack.Push(asset.Available.Value);

            return true;
        }

        protected virtual bool Asset_GetPrecision(IStackAccessor stack)
        {
            var asset = stack.Pop<Asset>();
            if (asset == null) return false;

            stack.Push((int)asset.Precision);

            return true;
        }

        protected virtual bool Asset_GetOwner(IStackAccessor stack)
        {
            var asset = stack.Pop<Asset>();
            if (asset == null) return false;

            // TODO: it was EncodePoint before. Check with NEO
            stack.Push(asset.Owner.EncodedData);

            return true;
        }

        protected virtual bool Asset_GetAdmin(IStackAccessor stack)
        {
            var asset = stack.Pop<Asset>();
            if (asset == null) return false;

            stack.Push(asset.Admin.ToArray());

            return true;
        }

        protected virtual bool Asset_GetIssuer(IStackAccessor stack)
        {
            var asset = stack.Pop<Asset>();
            if (asset == null) return false;

            stack.Push(asset.Issuer.ToArray());

            return true;
        }

        protected virtual bool Contract_GetScript(IStackAccessor stack)
        {
            var contract = stack.Pop<Contract>();
            if (contract == null) return false;

            stack.Push(contract.Script);

            return true;
        }

        protected virtual bool Contract_IsPayable(IStackAccessor stack)
        {
            var contract = stack.Pop<Contract>();
            if (contract == null) return false;

            stack.Push(contract.Payable);

            return true;
        }

        protected virtual bool Storage_GetContext(IStackAccessor stack)
        {
            stack.Push(new StorageContext
            {
                ScriptHash = stack.ScriptHash,
                IsReadOnly = false
            });

            return true;
        }

        protected virtual bool Storage_GetReadOnlyContext(IStackAccessor stack)
        {
            stack.Push(new StorageContext
            {
                ScriptHash = stack.ScriptHash,
                IsReadOnly = true
            });

            return true;
        }

        protected virtual bool Storage_Get(IStackAccessor stack)
        {
            var storageContext = stack.Pop<StorageContext>();
            if (storageContext == null) return false;
            if (!CheckStorageContext(storageContext)) return false;

            var key = stack.PopByteArray();
            var item = Storages.TryGet(new StorageKey
            {
                ScriptHash = storageContext.ScriptHash,
                Key = key
            });

            stack.Push(item?.Value ?? Array.Empty<byte>());

            return true;
        }

        protected virtual bool Storage_Find(IStackAccessor stack)
        {
            var storageContext = stack.Pop<StorageContext>();
            if (storageContext == null) return false;
            if (!CheckStorageContext(storageContext)) return false;

            var prefix = stack.PopByteArray();
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

            var iterator = Storages.Find(prefixKey)
                .Where(p => p.Key.Key.Take(prefix.Length).SequenceEqual(prefix))
                .GetEnumerator();

            stack.Push(iterator);
            _disposables.Add(iterator);

            return true;
        }

        protected virtual bool StorageContext_AsReadOnly(IStackAccessor stack)
        {
            var storageContext = stack.Pop<StorageContext>();
            if (storageContext == null) return false;
            if (!storageContext.IsReadOnly)
                storageContext = new StorageContext
                {
                    ScriptHash = storageContext.ScriptHash,
                    IsReadOnly = true
                };

            stack.Push(storageContext);

            return true;
        }

        //protected virtual bool Enumerator_Create(IStackAccessor stack)
        //{
        //    var array = stack.PopArray();
        //    if (array == null) return false;

        //    stack.Push(array.GetEnumerator());

        //    return true;
        //}

        //protected virtual bool Enumerator_Next(IStackAccessor stack)
        //{
        //    var enumerator = stack.Pop<IEnumerator>();
        //    if (enumerator == null) return false;

        //    enumerator.MoveNext();

        //    stack.Push(enumerator);

        //    return true;
        //}

        //protected virtual bool Enumerator_Value(IStackAccessor stack)
        //{
        //    var enumerator = stack.Pop<IEnumerator>();
        //    if (enumerator == null) return false;

        //    stack.Push(enumerator.Current);

        //    return true;
        //}

        //protected virtual bool Enumerator_Concat(IStackAccessor stack)
        //{
        //    var enumerator1 = stack.Pop<IEnumerator>();
        //    if (enumerator1 == null) return false;

        //    var enumerator2 = stack.Pop<IEnumerator>();
        //    if (enumerator2 == null) return false;

        //    IEnumerator result = new ConcatEnumerator(first, second);
        //    stack.Push(StackItem.FromInterface(result));
        //    return true;
        //}

        //protected virtual bool Iterator_Create(IStackAccessor stack)
        //{
        //    if (stack.Pop() is Map map)
        //    {
        //        IIterator iterator = new MapWrapper(map);
        //        stack.Push(StackItem.FromInterface(iterator));
        //        return true;
        //    }
        //    return false;
        //}

        //protected virtual bool Iterator_Key(IStackAccessor stack)
        //{
        //    if (stack.Pop() is InteropInterface _interface)
        //    {
        //        IIterator iterator = _interface.GetInterface<IIterator>();
        //        stack.Push(iterator.Key());
        //        return true;
        //    }
        //    return false;
        //}

        //protected virtual bool Iterator_Keys(IStackAccessor stack)
        //{
        //    if (stack.Pop() is InteropInterface _interface)
        //    {
        //        IIterator iterator = _interface.GetInterface<IIterator>();
        //        stack.Push(StackItem.FromInterface(new IteratorKeysWrapper(iterator)));
        //        return true;
        //    }
        //    return false;
        //}

        //protected virtual bool Iterator_Values(IStackAccessor stack)
        //{
        //    if (stack.Pop() is InteropInterface _interface)
        //    {
        //        IIterator iterator = _interface.GetInterface<IIterator>();
        //        stack.Push(StackItem.FromInterface(new IteratorValuesWrapper(iterator)));
        //        return true;
        //    }
        //    return false;
        //}
    }
}