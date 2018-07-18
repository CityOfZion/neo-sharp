using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Blockchain
{
    [TestClass]
    public class UtBlockchain : TestBase
    {
        [TestMethod]
        public void Ctor_CreateValidBlockchainObject()
        {
            var testee = this.AutoMockContainer.Create<Core.Blockchain.Blockchain>();

            testee
                .Should()
                .BeOfType<Core.Blockchain.Blockchain>();
        }

        [TestMethod]
        public async Task InitializeBlockchain_IsGenesisBlock_InitializeComplete()
        {
            var testee = this.AutoMockContainer.Create<Core.Blockchain.Blockchain>();
            await testee.InitializeBlockchain();

            // TODO: Need to verify all mock calls 
        }

        // TODO: InitializeBlockchain that don't create Genesis Block
    }
}
