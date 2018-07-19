using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Processors;
using NeoSharp.Core.Blockchain.State;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Blockchain.Processors
{
    [TestClass]
    public class UtTransactionProcessor : TestBase
    {
        [TestMethod]
        public async Task Process_ClaimTx_CallsClaimTxProcessor()
        {
            var input = new ClaimTransaction();
            var claimTxProcessorMock = AutoMockContainer.GetMock<IProcessor<ClaimTransaction>>();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
            claimTxProcessorMock.Verify(m => m.Process(input));
        }

        [TestMethod]
        public async Task Process_InvocationTx_CallsInvocationTxProcessor()
        {
            var input = new InvocationTransaction();
            var invocationTxProcessorMock = AutoMockContainer.GetMock<IProcessor<InvocationTransaction>>();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
            invocationTxProcessorMock.Verify(m => m.Process(input));
        }

        [TestMethod]
        public async Task Process_IssueTx_CallsIssueTxProcessor()
        {
            var input = new IssueTransaction();
            var issueTxProcessorMock = AutoMockContainer.GetMock<IProcessor<IssueTransaction>>();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
            issueTxProcessorMock.Verify(m => m.Process(input));
        }

        [TestMethod]
        public async Task Process_EnrollmentTx_CallsEnrollmentTxProcessor()
        {
            var input = new EnrollmentTransaction();
            var enrollmentTxProcessorMock = AutoMockContainer.GetMock<IProcessor<EnrollmentTransaction>>();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
            enrollmentTxProcessorMock.Verify(m => m.Process(input));
        }

        [TestMethod]
        public async Task Process_RegisterTx_CallsRegisterTxProcessor()
        {
            var input = new RegisterTransaction();
            var registerTxProcessorMock = AutoMockContainer.GetMock<IProcessor<RegisterTransaction>>();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
            registerTxProcessorMock.Verify(m => m.Process(input));
        }

        [TestMethod]
        public async Task Process_PublishTx_CallsPublishTxProcessor()
        {
            var input = new PublishTransaction();
            var publishTxProcessorMock = AutoMockContainer.GetMock<IProcessor<PublishTransaction>>();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
            publishTxProcessorMock.Verify(m => m.Process(input));
        }

        [TestMethod]
        public async Task Process_StateTx_CallsStateTxProcessor()
        {
            var input = new StateTransaction();
            var stateTxProcessorMock = AutoMockContainer.GetMock<IProcessor<StateTransaction>>();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
            stateTxProcessorMock.Verify(m => m.Process(input));
        }

        [TestMethod]
        public async Task Process_ContractTx_NoSpecialCalls()
        {
            var input = new ContractTransaction();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
        }

        [TestMethod]
        public async Task Process_MinerTx_NoSpecialCalls()
        {
            var input = new MinerTransaction();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task Process_Transaction_Errors()
        {
            var input = new Transaction();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
        }

        [TestMethod]
        public async Task Process_TransactionWithOutputs_GainOutputs()
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
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);

            foreach (var expectedOutput in input.Outputs)
                accountManagerMock.Verify(m =>
                    m.UpdateBalance(expectedOutput.ScriptHash, expectedOutput.AssetId, expectedOutput.Value));
            repositoryMock.Verify(m => m.AddCoinStates(input.Hash,
                It.Is<CoinState[]>(cs => cs.Length == input.Outputs.Length && cs.All(c => c.Equals(CoinState.New)))));
        }

        [TestMethod]
        public async Task Process_TransactionWithInputs_SpendOutputs()
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
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
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
        public async Task Process_Transaction_SaveTx()
        {
            var input = new ContractTransaction();
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            var testee = AutoMockContainer.Create<TransactionProcessor>();

            await testee.Process(input);
            repositoryMock.Verify(m => m.AddTransaction(input));

        }
    }
}