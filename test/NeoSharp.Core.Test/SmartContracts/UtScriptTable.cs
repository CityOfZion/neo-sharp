using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.SmartContract;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.SmartContracts
{
    [TestClass]
    public class UtScriptTable : TestBase
    {
        class dummyRepo : IRepository
        {
            public Task AddAccount(Account acct)
            {
                throw new System.NotImplementedException();
            }

            public Task AddAsset(Asset asset)
            {
                throw new System.NotImplementedException();
            }

            public Task AddBlockHeader(BlockHeader blockHeader)
            {
                throw new System.NotImplementedException();
            }

            public Task AddCoinStates(UInt256 txHash, CoinState[] coinStates)
            {
                throw new System.NotImplementedException();
            }

            public Task AddContract(Contract contract)
            {
                throw new System.NotImplementedException();
            }

            public Task AddStorage(StorageKey key, StorageValue val)
            {
                throw new System.NotImplementedException();
            }

            public Task AddTransaction(Transaction transaction)
            {
                throw new System.NotImplementedException();
            }

            public Task AddValidator(Validator validator)
            {
                throw new System.NotImplementedException();
            }

            public Task DeleteAccount(UInt160 hash)
            {
                throw new System.NotImplementedException();
            }

            public Task DeleteAsset(UInt256 assetId)
            {
                throw new System.NotImplementedException();
            }

            public Task DeleteCoinStates(UInt256 txHash)
            {
                throw new System.NotImplementedException();
            }

            public Task DeleteContract(UInt160 contractHash)
            {
                throw new System.NotImplementedException();
            }

            public Task DeleteStorage(StorageKey key)
            {
                throw new System.NotImplementedException();
            }

            public Task DeleteValidator(ECPoint point)
            {
                throw new System.NotImplementedException();
            }

            public Task<Account> GetAccount(UInt160 hash)
            {
                throw new System.NotImplementedException();
            }

            public Task<Asset> GetAsset(UInt256 assetId)
            {
                throw new System.NotImplementedException();
            }

            public Task<IEnumerable<UInt256>> GetBlockHashesFromHeights(IEnumerable<uint> heights)
            {
                throw new System.NotImplementedException();
            }

            public Task<UInt256> GetBlockHashFromHeight(uint height)
            {
                throw new System.NotImplementedException();
            }

            public Task<BlockHeader> GetBlockHeader(UInt256 hash)
            {
                throw new System.NotImplementedException();
            }

            public Task<CoinState[]> GetCoinStates(UInt256 txHash)
            {
                throw new System.NotImplementedException();
            }

            public Task<Contract> GetContract(UInt160 contractHash)
            {
                if (contractHash.Equals(UInt160.Zero))
                {
                    return Task.FromResult(new Contract()
                    {
                        Code = new Code()
                        {
                            Script = new byte[] { 0x01, 0x02, 0x03 },
                        }
                    });
                }

                return null;
            }

            public Task<HashSet<CoinReference>> GetIndexClaimable(UInt160 scriptHash)
            {
                throw new System.NotImplementedException();
            }

            public Task<HashSet<CoinReference>> GetIndexConfirmed(UInt160 scriptHash)
            {
                throw new System.NotImplementedException();
            }

            public Task<uint> GetIndexHeight()
            {
                throw new System.NotImplementedException();
            }

            public Task<StorageValue> GetStorage(StorageKey key)
            {
                throw new System.NotImplementedException();
            }

            public Task<uint> GetTotalBlockHeaderHeight()
            {
                throw new System.NotImplementedException();
            }

            public Task<uint> GetTotalBlockHeight()
            {
                throw new System.NotImplementedException();
            }

            public Task<uint> GetTransactionHeightFromHash(UInt256 hash)
            {
                throw new System.NotImplementedException();
            }

            public Task<Transaction> GetTransaction(UInt256 hash)
            {
                throw new System.NotImplementedException();
            }

            public Task<Validator> GetValidator(ECPoint publicKey)
            {
                throw new System.NotImplementedException();
            }

            public Task<string> GetVersion()
            {
                throw new System.NotImplementedException();
            }

            public Task SetIndexClaimable(UInt160 scriptHash, HashSet<CoinReference> coinReferences)
            {
                throw new System.NotImplementedException();
            }

            public Task SetIndexConfirmed(UInt160 scriptHash, HashSet<CoinReference> coinReferences)
            {
                throw new System.NotImplementedException();
            }

            public Task SetIndexHeight(uint height)
            {
                throw new System.NotImplementedException();
            }

            public Task SetTotalBlockHeaderHeight(uint height)
            {
                throw new System.NotImplementedException();
            }

            public Task SetTotalBlockHeight(uint height)
            {
                throw new System.NotImplementedException();
            }

            public Task SetVersion(string version)
            {
                throw new System.NotImplementedException();
            }
        }

        [TestMethod]
        public void TestMessageContainer()
        {
            var data = RandomByteArray(250);
            var msg = new ScriptTable(new dummyRepo());

            var script = msg.GetScript(UInt160.Parse("0x0d7c041c584acea51d66f1e9bf144477ad6dca25").ToArray(), false);

            Assert.IsNull(script);

            script = msg.GetScript(UInt160.Zero.ToArray(), false);

            Assert.IsTrue(script.SequenceEqual(new byte[] { 0x01, 0x02, 0x03 }));
        }
    }
}