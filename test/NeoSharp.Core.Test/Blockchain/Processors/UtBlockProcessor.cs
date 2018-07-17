using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Processors;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Blockchain.Processors
{
    [TestClass]
    public class UtBlockProcessor : TestBase
    {
        [TestMethod]
        public async Task Process_CallsTransactionProcessorPerTransaction()
        {
            var input = new Block
            {
                Index = (uint) RandomInt(),
                Transactions = new[]
                {
                    new Transaction(),
                    new Transaction()
                }
            };
            var txProcessorMock = AutoMockContainer.GetMock<IProcessor<Transaction>>();
            var testee = AutoMockContainer.Create<BlockProcessor>();

            await testee.Process(input);
            txProcessorMock.Verify(m => m.Process(It.Is<Transaction>(t => input.Transactions.Contains(t))),
                Times.Exactly(input.Transactions.Length));
        }


        [TestMethod]
        public async Task Process_PersistsBlockHeaderAndIndex()
        {
            var expectedIndex = (uint) RandomInt();
            var input = new Block
            {
                Hash = UInt256.Parse(RandomInt().ToString("X64")),
                Index = expectedIndex,
                Transactions = new Transaction[0]
            };
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            var testee = AutoMockContainer.Create<BlockProcessor>();

            await testee.Process(input);
            repositoryMock.Verify(m => m.SetTotalBlockHeight(expectedIndex));
            repositoryMock.Verify(m =>
                m.AddBlockHeader(It.Is<BlockHeader>(bh => bh.Index == input.Index && bh.Hash.Equals(input.Hash))));
        }
    }
}