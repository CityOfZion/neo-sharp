using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Persistence.RocksDB.Tests
{
    [TestClass]
    public class RocksDbModelTests : TestBase
    {
        [TestMethod]
        public void Create_ValidTEntity_KeyAndTEntitySerializedAndSendToContext()
        {
            var entity = new BlockHeader
            {
                Hash = UInt256.Zero
            };

            var key = entity.Hash.BuildKey(DataEntryPrefix.DataBlock);
            var serializedTEntity = new byte[] { };

            var serializeMock = this.AutoMockContainer.GetMock<IBinarySerializer>();
            serializeMock
                .Setup(x => x.Serialize(entity, null))
                .Returns(serializedTEntity);

            var dbContextMock = this.AutoMockContainer.GetMock<IDbContext>();

            var testee = this.AutoMockContainer.Create<RocksDbModel>();
            testee.Create(DataEntryPrefix.DataBlock, entity.Hash, entity);

            dbContextMock.Verify(x => x.Create(key, serializedTEntity));
        }
    }
}