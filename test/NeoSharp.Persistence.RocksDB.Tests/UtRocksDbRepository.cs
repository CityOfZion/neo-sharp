using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Types;
using NeoSharp.TestHelpers;

namespace NeoSharp.Persistence.RocksDB.Tests
{
    [TestClass]
    public class UtRocksDbRepository : TestBase
    {
        [TestMethod]
        public void Ctor_CreateValidRocksDbRepository()
        {
            var testee = this.AutoMockContainer.Create<RocksDbRepository>();

            testee.Should().BeOfType<RocksDbRepository>();
        }

        [TestMethod]
        public async Task GetBlockHashFromHeight_NoHashFound_ReturnUInt256Zero()
        {
            // Arrange
            const uint heightParameter = 0;

            var rocksDbContextMock = this.AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(heightParameter.BuildIxHeightToHashKey()))
                .Returns(() => Task.FromResult<byte[]>(null));

            var testee = this.AutoMockContainer.Create<RocksDbRepository>();

            // Act
            var result = await testee.GetBlockHashFromHeight(heightParameter);

            // Assert
            result
                .Should()
                .Be(UInt256.Zero);
        }

        [TestMethod]
        public async Task GetBlockHashFromHeight_HashFound_ReturnValidUInt256()
        {
            // Arrange
            const uint heightParameter = 0;
            var heightByteArray = heightParameter.BuildIxHeightToHashKey();
            var byteArrayParameter = new byte[] { 157, 179, 60, 8, 66, 122, 255, 105, 126, 49, 180, 74, 212, 41, 126, 177, 14, 255, 59, 82, 218, 113, 248, 145, 98, 5, 128, 140, 42, 70, 32, 69 };
            var expectedUint256 = UInt256.Parse("0x4520462a8c80056291f871da523bff0eb17e29d44ab4317e69ff7a42083cb39d");

            var rocksDbContextMock = this.AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(It.Is<byte[]>(a => a.Where((b, i) => b == heightByteArray[i]).Count() == heightByteArray.Length)))
                .Returns(() => Task.FromResult(byteArrayParameter));

            var testee = this.AutoMockContainer.Create<RocksDbRepository>();

            // Act
            var result = await testee.GetBlockHashFromHeight(heightParameter);

            // Assert
            result
                .Should()
                .BeEquivalentTo(expectedUint256);
        }

        [TestMethod]
        public async Task AddBlockHeader_ValidBlockHeader_BlockHeaderAndHeightHashSaved()
        {
            // Arrange
            var expectedBlockByteArray = new byte[] { 157, 179, 60, 8, 66, 122, 255, 105, 126, 49, 180, 74, 212, 41, 126, 177, 14, 255, 59, 82, 218, 113, 248, 145, 98, 5, 128, 140, 42, 70, 32, 69, 0 };
            var hash = UInt256.Parse("0x4520462a8c80056291f871da523bff0eb17e29d44ab4317e69ff7a42083cb39d");
            var hashKeyByteArray = hash.BuildDataBlockKey();
            const uint index = 0;
            var indexByteArray = index.BuildIxHeightToHashKey();

            var blockHeaderParameter = new BlockHeaderBase
            {
                Hash = hash,
                Index = index
            };

            var serializerMock = this.AutoMockContainer.GetMock<IBinarySerializer>();
            serializerMock
                .Setup(x => x.Serialize(blockHeaderParameter, null))
                .Returns(expectedBlockByteArray);

            var rocksDbContextMock = this.AutoMockContainer.GetMock<IRocksDbContext>();

            var testee = this.AutoMockContainer.Create<RocksDbRepository>();

            // Act
            await testee.AddBlockHeader(blockHeaderParameter);

            // Assert
            rocksDbContextMock.Verify(x => x.Save(
                    It.IsAny<byte[]>(),
                    It.IsAny<byte[]>()),
                Times.Exactly(2));

            rocksDbContextMock.Verify(x => x.Save(
                It.Is<byte[]>(a => a.ArrayIsEquivalentTo(hashKeyByteArray)),
                It.Is<byte[]>(a => a.ArrayIsEquivalentTo(expectedBlockByteArray))),
                Times.Once);

            rocksDbContextMock.Verify(x => x.Save(
                It.Is<byte[]>(a => a.ArrayIsEquivalentTo(indexByteArray)),
                It.Is<byte[]>(a => a.ArrayIsEquivalentTo(hash.ToArray()))),
                Times.Once);

        }
    }
}
