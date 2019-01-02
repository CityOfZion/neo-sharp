using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NeoSharp.Core.Blockchain.Repositories;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Models;
using NeoSharp.Core.Models.OperationManager;
using NeoSharp.Core.Network;
using NeoSharp.Core.SmartContract;
using NeoSharp.Types;
using NeoSharp.VM;

namespace NeoSharp.Core.VM
{
    public class StateMachine : StateReader
    {
        private readonly Block _persistingBlock;
        private readonly IBlockchainContext _blockchainContext;

        private readonly Dictionary<UInt160, UInt160> _contractsCreated = new Dictionary<UInt160, UInt160>();

        public StateMachine(
            Block persistingBlock,
            DataCache<UInt160, Account> accounts,
            DataCache<UInt256, Asset> assets,
            DataCache<UInt160, Contract> contracts,
            DataCache<StorageKey, StorageValue> storages,
            InteropService interopService,
            IBlockchainContext blockchainContext,
            IBlockRepository blockRepository,
            ITransactionRepository transactionRepository,
            ITransactionOperationsManager transactionOperationsManager)
        : base
            (
            accounts?.CreateSnapshot(),
            assets?.CreateSnapshot(),
            contracts?.CreateSnapshot(),
            storages?.CreateSnapshot(),
            interopService,
            blockchainContext,
            blockRepository,
            transactionRepository,
            transactionOperationsManager
            )
        {
            _persistingBlock = persistingBlock;
            _blockchainContext = blockchainContext;

            // Standard Library

            interopService.Register("System.Contract.GetStorageContext", Contract_GetStorageContext);
            interopService.Register("System.Contract.Destroy", Contract_Destroy);
            interopService.Register("System.Storage.Put", Storage_Put);
            interopService.Register("System.Storage.Delete", Storage_Delete);

            // Neo Specified

            interopService.Register("Neo.Asset.Create", Asset_Create);
            interopService.Register("Neo.Asset.Renew", Asset_Renew);
            interopService.Register("Neo.Contract.Create", Contract_Create);
            interopService.Register("Neo.Contract.Migrate", Contract_Migrate);

            #region Old APIs

            interopService.Register("AntShares.Asset.Create", Asset_Create);
            interopService.Register("AntShares.Asset.Renew", Asset_Renew);
            interopService.Register("AntShares.Contract.Create", Contract_Create);
            interopService.Register("AntShares.Contract.Migrate", Contract_Migrate);
            interopService.Register("Neo.Contract.GetStorageContext", Contract_GetStorageContext);
            interopService.Register("AntShares.Contract.GetStorageContext", Contract_GetStorageContext);
            interopService.Register("Neo.Contract.Destroy", Contract_Destroy);
            interopService.Register("AntShares.Contract.Destroy", Contract_Destroy);
            interopService.Register("Neo.Storage.Put", Storage_Put);
            interopService.Register("AntShares.Storage.Put", Storage_Put);
            interopService.Register("Neo.Storage.Delete", Storage_Delete);
            interopService.Register("AntShares.Storage.Delete", Storage_Delete);

            #endregion
        }

        public void Commit()
        {
            Accounts.Commit();
            Assets.Commit();
            Contracts.Commit();
            Storages.Commit();
        }

        protected override bool Runtime_GetTime(IExecutionEngine engine)
        {
            engine.CurrentContext.EvaluationStack.Push(_persistingBlock.Timestamp);
            return true;
        }

        private bool Asset_Create(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var tx = (InvocationTransaction)engine.MessageProvider.GetMessage(0);
            var assetType = (AssetType)(byte)stack.PopBigInteger();
            if (!Enum.IsDefined(typeof(AssetType), assetType) || assetType == AssetType.CreditFlag || assetType == AssetType.DutyFlag || assetType == AssetType.GoverningToken || assetType == AssetType.UtilityToken)
                return false;
            if (stack.PeekByteArray().Length > 1024)
                return false;

            var name = Encoding.UTF8.GetString(stack.PopByteArray());
            var amount = new Fixed8((long)stack.PopBigInteger());
            if (amount == Fixed8.Zero || amount < -Fixed8.Satoshi) return false;
            if (assetType == AssetType.Invoice && amount != -Fixed8.Satoshi)
                return false;

            var precision = (byte)stack.PopBigInteger();
            if (precision > 8) return false;
            if (assetType == AssetType.Share && precision != 0) return false;
            if (amount != -Fixed8.Satoshi && amount.Value % (long)Math.Pow(10, 8 - precision) != 0)
                return false;

            var owner = new ECPoint(stack.PopByteArray());
            if (owner.IsInfinity) return false;
            if (!CheckWitness(engine, owner))
                return false;

            var admin = new UInt160(stack.PopByteArray());
            var issuer = new UInt160(stack.PopByteArray());
            var asset = Assets.GetOrAdd(tx.Hash, () => new Asset
            {
                Id = tx.Hash,
                AssetType = assetType,
                Name = name,
                Amount = amount,
                Available = Fixed8.Zero,
                Precision = precision,
                //Fee = Fixed8.Zero,
                //FeeAddress = new UInt160(),
                Owner = owner,
                Admin = admin,
                Issuer = issuer,
                Expiration = _blockchainContext.CurrentBlock.Index + 1 + 2000000,
                IsFrozen = false
            });

            stack.PushObject(asset);

            return true;
        }

        private bool Asset_Renew(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var asset = stack.PopObject<Asset>();
            if (asset == null) return false;

            var years = (byte)stack.PopBigInteger();

            asset = Assets.GetAndChange(asset.Id);

            if (asset.Expiration < _blockchainContext.CurrentBlock.Index + 1)
                asset.Expiration = _blockchainContext.CurrentBlock.Index + 1;

            try
            {
                asset.Expiration = checked(asset.Expiration + years * 2000000u);
            }
            catch (OverflowException)
            {
                asset.Expiration = uint.MaxValue;
            }

            stack.Push(asset.Expiration);

            return true;
        }

        private bool Contract_Create(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var script = stack.PopByteArray();
            if (script.Length > 1024 * 1024) return false;

            var parameters = stack.PopByteArray().Select(p => (ContractParameterType)p).ToArray();
            if (parameters.Length > 252) return false;

            var returnType = (ContractParameterType)(byte)stack.PopBigInteger();
            var metadata = (ContractMetadata)(byte)stack.PopBigInteger();

            if (stack.PeekByteArray().Length > 252) return false;
            var name = Encoding.UTF8.GetString(stack.PopByteArray());

            if (stack.PeekByteArray().Length > 252) return false;
            var version = Encoding.UTF8.GetString(stack.PopByteArray());

            if (stack.PeekByteArray().Length > 252) return false;
            var author = Encoding.UTF8.GetString(stack.PopByteArray());

            if (stack.PeekByteArray().Length > 252) return false;
            var email = Encoding.UTF8.GetString(stack.PopByteArray());

            if (stack.PeekByteArray().Length > 65536) return false;
            var description = Encoding.UTF8.GetString(stack.PopByteArray());

            var scriptHash = script.ToScriptHash();
            var contract = Contracts.TryGet(scriptHash);
            if (contract == null)
            {
                contract = new Contract
                {
                    Code = new Code
                    {
                        Script = script,
                        ScriptHash = scriptHash,
                        Parameters = parameters,
                        ReturnType = returnType,
                        Metadata = metadata
                    },
                    Name = name,
                    Version = version,
                    Author = author,
                    Email = email,
                    Description = description
                };

                Contracts.Add(scriptHash, contract);
                _contractsCreated.Add(scriptHash, new UInt160(engine.CurrentContext.ScriptHash));
            }

            stack.PushObject(contract);

            return true;
        }

        private bool Contract_Migrate(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var script = stack.PopByteArray();
            if (script.Length > 1024 * 1024) return false;

            var parameters = stack.PopByteArray().Select(p => (ContractParameterType)p).ToArray();
            if (parameters.Length > 252) return false;

            var returnType = (ContractParameterType)(byte)stack.PopBigInteger();
            var metadata = (ContractMetadata)(byte)stack.PopBigInteger();

            if (stack.PeekByteArray().Length > 252) return false;
            var name = Encoding.UTF8.GetString(stack.PopByteArray());

            if (stack.PeekByteArray().Length > 252) return false;
            var version = Encoding.UTF8.GetString(stack.PopByteArray());

            if (stack.PeekByteArray().Length > 252) return false;
            var author = Encoding.UTF8.GetString(stack.PopByteArray());

            if (stack.PeekByteArray().Length > 252) return false;
            var email = Encoding.UTF8.GetString(stack.PopByteArray());

            if (stack.PeekByteArray().Length > 65536) return false;
            var description = Encoding.UTF8.GetString(stack.PopByteArray());

            var scriptHash = script.ToScriptHash();
            var contract = Contracts.TryGet(scriptHash);
            if (contract == null)
            {
                contract = new Contract
                {
                    Code = new Code
                    {
                        Script = script,
                        ScriptHash = scriptHash,
                        Parameters = parameters,
                        ReturnType = returnType,
                        Metadata = metadata
                    },
                    Name = name,
                    Version = version,
                    Author = author,
                    Email = email,
                    Description = description
                };

                Contracts.Add(scriptHash, contract);
                _contractsCreated.Add(scriptHash, new UInt160(engine.CurrentContext.ScriptHash));
                if (contract.HasStorage)
                {
                    foreach (var pair in Storages.Find(engine.CurrentContext.ScriptHash).ToArray())
                    {
                        Storages.Add(new StorageKey
                        {
                            ScriptHash = scriptHash,
                            Key = pair.Key.Key
                        }, new StorageValue
                        {
                            Value = pair.Value.Value
                        });
                    }
                }
            }

            stack.PushObject(contract);

            return Contract_Destroy(engine);
        }

        private bool Contract_GetStorageContext(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var contract = stack.PopObject<Contract>();
            if (!_contractsCreated.TryGetValue(contract.ScriptHash, out var created)) return false;
            if (!created.Equals(new UInt160(engine.CurrentContext.ScriptHash))) return false;

            stack.PushObject(new StorageContext
            (
                contract.ScriptHash,
                false
            ));

            return true;
        }

        private bool Contract_Destroy(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var hash = new UInt160(engine.CurrentContext.ScriptHash);
            var contract = Contracts.TryGet(hash);
            if (contract == null) return true;

            Contracts.Delete(hash);

            if (contract.HasStorage)
                foreach (var pair in Storages.Find(hash.ToArray()))
                    Storages.Delete(pair.Key);

            return true;
        }

        private bool Storage_Put(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var context = stack.PopObject<StorageContext>();
            if (context == null) return false;
            if (context.IsReadOnly) return false;
            if (!CheckStorageContext(context)) return false;

            var key = stack.PopByteArray();
            if (key.Length > 1024) return false;

            var value = stack.PopByteArray();

            Storages.GetAndChange(new StorageKey
            {
                ScriptHash = context.ScriptHash,
                Key = key
            }, () => new StorageValue()).Value = value;

            return true;
        }

        private bool Storage_Delete(IExecutionEngine engine)
        {
            var stack = engine.CurrentContext.EvaluationStack;
            var context = stack.PopObject<StorageContext>();
            if (context == null) return false;
            if (context.IsReadOnly) return false;
            if (!CheckStorageContext(context)) return false;

            var key = stack.PopByteArray();

            Storages.Delete(new StorageKey
            {
                ScriptHash = context.ScriptHash,
                Key = key
            });

            return true;
        }
    }
}