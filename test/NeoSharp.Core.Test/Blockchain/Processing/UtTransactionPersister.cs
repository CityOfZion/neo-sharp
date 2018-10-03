using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Blockchain.State;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Blockchain.Processing
{
    [TestClass]
    public class UtTransactionPersister : TestBase
    {
        [TestMethod]
        public async Task Persist_ClaimTx_CallsClaimTxPersister()
        {
            var input = new ClaimTransaction();
            var claimTxPersisterMock = AutoMockContainer.GetMock<ITransactionPersister<ClaimTransaction>>();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
            claimTxPersisterMock.Verify(m => m.Persist(input));
        }

        [TestMethod]
        public async Task Persist_InvocationTx_CallsInvocationTxPersister()
        {
            var input = new InvocationTransaction();
            var invocationTxPersisterMock = AutoMockContainer.GetMock<ITransactionPersister<InvocationTransaction>>();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
            invocationTxPersisterMock.Verify(m => m.Persist(input));
        }

        [TestMethod]
        public async Task Persist_IssueTx_CallsIssueTxPersister()
        {
            var input = new IssueTransaction();
            var issueTxPersisterMock = AutoMockContainer.GetMock<ITransactionPersister<IssueTransaction>>();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
            issueTxPersisterMock.Verify(m => m.Persist(input));
        }

        [TestMethod]
        public async Task Persist_EnrollmentTx_CallsEnrollmentTxPersister()
        {
            var input = new EnrollmentTransaction();
            var enrollmentTxPersisterMock = AutoMockContainer.GetMock<ITransactionPersister<EnrollmentTransaction>>();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
            enrollmentTxPersisterMock.Verify(m => m.Persist(input));
        }

        [TestMethod]
        public async Task Persist_RegisterTx_CallsRegisterTxPersister()
        {
            var input = new RegisterTransaction();
            var registerTxPersisterMock = AutoMockContainer.GetMock<ITransactionPersister<RegisterTransaction>>();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
            registerTxPersisterMock.Verify(m => m.Persist(input));
        }

        [TestMethod]
        public async Task Persist_PublishTx_CallsPublishTxPersister()
        {
            var input = new PublishTransaction();
            var publishTxPersisterMock = AutoMockContainer.GetMock<ITransactionPersister<PublishTransaction>>();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
            publishTxPersisterMock.Verify(m => m.Persist(input));
        }

        [TestMethod]
        public async Task Persist_StateTx_CallsStateTxPersister()
        {
            var input = new StateTransaction();
            var stateTxPersisterMock = AutoMockContainer.GetMock<ITransactionPersister<StateTransaction>>();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
            stateTxPersisterMock.Verify(m => m.Persist(input));
        }

        [TestMethod]
        public async Task Persist_ContractTx_NoSpecialCalls()
        {
            var input = new ContractTransaction();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
        }

        [TestMethod]
        public async Task Persist_MinerTx_NoSpecialCalls()
        {
            var input = new MinerTransaction();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Persist_Transaction_Errors()
        {
            var input = new Transaction();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
        }

        [TestMethod]
        public async Task Persist_TransactionWithOutputs_GainOutputs()
        {
            var input = new ContractTransaction
            {
                Hash = UInt256.Parse(RandomInt().ToString("X64")),
                Outputs = new TransactionOutput[3]
            };
            for (var i = 0; i < input.Outputs.Length; i++)
                input.Outputs[i] = new TransactionOutput
                {
                    AssetId = UInt256.Parse(RandomInt().ToString("X64")),
                    ScriptHash = UInt160.Parse(RandomInt().ToString("X40")),
                    Value = new Fixed8(RandomInt())
                };
            var accountManagerMock = AutoMockContainer.GetMock<IAccountManager>();
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);

            foreach (var expectedOutput in input.Outputs)
                accountManagerMock.Verify(m =>
                    m.UpdateBalance(expectedOutput.ScriptHash, expectedOutput.AssetId, expectedOutput.Value));
            repositoryMock.Verify(m => m.AddCoinStates(input.Hash,
                It.Is<CoinState[]>(cs => cs.Length == input.Outputs.Length && cs.All(c => c.Equals(CoinState.New)))));
        }

        [TestMethod]
        public async Task Persist_TransactionWithInputs_SpendOutputs()
        {
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            var input = new ContractTransaction
            {
                Hash = UInt256.Parse(RandomInt().ToString("X64")),
                Inputs = new CoinReference[3]
            };
            var txs = new Transaction[3];
            for (var i = 0; i < input.Inputs.Length; i++)
            {
                var reference = new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = 0
                };
                input.Inputs[i] = reference;
                txs[i] = new Transaction
                {
                    Hash = reference.PrevHash,
                    Outputs = new[]
                    {
                        new TransactionOutput
                        {
                            AssetId = UInt256.Parse(RandomInt().ToString("X64")),
                            ScriptHash = UInt160.Parse(RandomInt().ToString("X40")),
                            Value = new Fixed8(RandomInt())
                        }
                    }
                };
                repositoryMock
                    .Setup(m => m.GetTransaction(reference.PrevHash))
                    .ReturnsAsync(txs[i]);
                repositoryMock
                    .Setup(m => m.GetCoinStates(reference.PrevHash))
                    .ReturnsAsync(new[] {CoinState.Confirmed});
            }

            var accountManagerMock = AutoMockContainer.GetMock<IAccountManager>();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
            for (var i = 0; i < input.Outputs.Length; i++)
            {
                var output = txs[i].Outputs[0];
                accountManagerMock.Verify(m => m.UpdateBalance(output.ScriptHash, output.AssetId, -output.Value));
                var hash = txs[i].Hash;
                repositoryMock.Verify(m => m.AddCoinStates(It.Is<UInt256>(u => u.Equals(hash)),
                    It.Is<CoinState[]>(cs => cs.Length == 1 && cs[0].Equals(CoinState.Confirmed | CoinState.Spent))));
            }
        }

        [TestMethod]
        public async Task Persist_Transaction_SaveTx()
        {
            var input = new ContractTransaction();
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            var testee = AutoMockContainer.Create<TransactionPersister>();

            await testee.Persist(input);
            repositoryMock.Verify(m => m.AddTransaction(input));

        }
    }
}