using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Persistence.Contexts;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Persistence
{
    [TestClass]
    public class BlockHeaderContextTests : TestBase
    {
        [TestMethod]
        public void Add_AddBlockheaderToRepository()
        {
            var blockHeader = new BlockHeader
            {
                Hash = new UInt256(new byte[] { 157, 179, 60, 8, 66, 122, 255, 105, 126, 49, 180, 74, 212, 41, 126, 177, 14, 255, 59, 82, 218, 113, 248, 145, 98, 5, 128, 140, 42, 70, 32, 69 })
            };

            var dbModelMock = this.AutoMockContainer.GetMock<IDbModel>();

            var testee = this.AutoMockContainer.Create<BlockHeaderContext>();
            testee.Add(blockHeader);

            dbModelMock.Verify(x => x.Create(DataEntryPrefix.DataBlock, blockHeader.Hash, blockHeader));
        }
    }
}
