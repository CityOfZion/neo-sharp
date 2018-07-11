using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
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
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            testee.Should().BeOfType<RocksDbRepository>();
        }

        [TestMethod]
        public async Task GetBlockHashFromHeight_NoHashFound_ReturnUInt256Zero()
        {
            // Arrange
            const uint heightParameter = 0;

            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(heightParameter.BuildIxHeightToHashKey()))
                .Returns(() => Task.FromResult<byte[]>(null));

            var testee = AutoMockContainer.Create<RocksDbRepository>();

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
            var byteArrayParameter = new byte[]
            {
                157, 179, 60, 8, 66, 122, 255, 105, 126, 49, 180, 74, 212, 41, 126, 177, 14, 255, 59, 82, 218, 113, 248,
                145, 98, 5, 128, 140, 42, 70, 32, 69
            };
            var expectedUint256 = UInt256.Parse("0x4520462a8c80056291f871da523bff0eb17e29d44ab4317e69ff7a42083cb39d");

            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(It.Is<byte[]>(a =>
                    a.Where((b, i) => b == heightByteArray[i]).Count() == heightByteArray.Length)))
                .Returns(() => Task.FromResult(byteArrayParameter));

            var testee = AutoMockContainer.Create<RocksDbRepository>();

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
            var expectedBlockByteArray = new byte[]
            {
                157, 179, 60, 8, 66, 122, 255, 105, 126, 49, 180, 74, 212, 41, 126, 177, 14, 255, 59, 82, 218, 113, 248,
                145, 98, 5, 128, 140, 42, 70, 32, 69, 0
            };
            var hash = UInt256.Parse("0x4520462a8c80056291f871da523bff0eb17e29d44ab4317e69ff7a42083cb39d");
            var hashKeyByteArray = hash.BuildDataBlockKey();
            const uint index = 0;
            var indexByteArray = index.BuildIxHeightToHashKey();

            var blockHeaderParameter = new BlockHeaderBase
            {
                Hash = hash,
                Index = index
            };

            var serializerMock = AutoMockContainer.GetMock<IBinarySerializer>();
            serializerMock
                .Setup(x => x.Serialize(blockHeaderParameter, null))
                .Returns(expectedBlockByteArray);

            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();

            var testee = AutoMockContainer.Create<RocksDbRepository>();

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

        [TestMethod]
        public async Task GetIndexHeight_NoValueFound_ReturnsUIntMinValue()
        {
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.Length == 1 && b[0] == (byte) DataEntryPrefix.IxIndexHeight)))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetIndexHeight();

            result
                .Should()
                .Be(uint.MinValue);
        }

        [TestMethod]
        public async Task GetIndexHeight_ValueFound_ReturnsUint()
        {
            var expectedHeight = (uint) RandomInt();
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.Length == 1 && b[0] == (byte) DataEntryPrefix.IxIndexHeight)))
                .ReturnsAsync(BitConverter.GetBytes(expectedHeight));
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetIndexHeight();

            result.Should().Be(expectedHeight);
        }

        [TestMethod]
        public async Task GetIndexConfirmed_NoValueFound_ReturnsEmptySet()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(input.BuildIndexConfirmedKey()))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetIndexConfirmed(input);

            result.Should().BeOfType<HashSet<CoinReference>>();
            result.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task GetIndexConfirmed_ValueFound_ReturnsFilledSet()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedBytes = new byte[1];
            var expectedReferences = new List<CoinReference>
            {
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = (ushort) RandomInt(10)
                },
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = (ushort) RandomInt(10)
                }
            }.ToArray();

            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildIndexConfirmedKey()))))
                .ReturnsAsync(expectedBytes);
            var deserializerMock = AutoMockContainer.GetMock<IBinaryDeserializer>();
            deserializerMock
                .Setup(m => m.Deserialize<CoinReference[]>(expectedBytes, null))
                .Returns(expectedReferences);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetIndexConfirmed(input);

            result.Should().BeOfType<HashSet<CoinReference>>();
            result.Count.Should().Be(2);
            result.Contains(expectedReferences[0]).Should().BeTrue();
            result.Contains(expectedReferences[1]).Should().BeTrue();
        }

        [TestMethod]
        public async Task SetIndexConfirmed_WritesCorrectKeyValue()
        {
            var inputHash = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedKey = inputHash.BuildIndexConfirmedKey();
            var inputReferences = new List<CoinReference>
            {
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = (ushort) RandomInt(10)
                },
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = (ushort) RandomInt(10)
                }
            }.ToArray();
            var inputSet = inputReferences.ToHashSet();
            var expectedBytes = new byte[1];
            var serializerMock = AutoMockContainer.GetMock<IBinarySerializer>();
            serializerMock
                .Setup(m => m.Serialize(It.Is<CoinReference[]>(arr => arr.SequenceEqual(inputReferences)), null))
                .Returns(expectedBytes);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.SetIndexConfirmed(inputHash, inputSet);

            rocksDbContextMock.Verify(m => m.Save(expectedKey, expectedBytes));
        }

        [TestMethod]
        public async Task GetIndexClaimable_NoValueFound_ReturnsEmptySet()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(input.BuildIndexClaimableKey()))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetIndexClaimable(input);

            result.Should().BeOfType<HashSet<CoinReference>>();
            result.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task GetIndexClaimable_ValueFound_ReturnsFilledSet()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedBytes = new byte[1];
            var expectedReferences = new List<CoinReference>
            {
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = (ushort) RandomInt(10)
                },
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = (ushort) RandomInt(10)
                }
            }.ToArray();

            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildIndexClaimableKey()))))
                .ReturnsAsync(expectedBytes);
            var deserializerMock = AutoMockContainer.GetMock<IBinaryDeserializer>();
            deserializerMock
                .Setup(m => m.Deserialize<CoinReference[]>(expectedBytes, null))
                .Returns(expectedReferences);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetIndexClaimable(input);

            result.Should().BeOfType<HashSet<CoinReference>>();
            result.Count.Should().Be(2);
            result.Contains(expectedReferences[0]).Should().BeTrue();
            result.Contains(expectedReferences[1]).Should().BeTrue();
        }

        [TestMethod]
        public async Task SetIndexClaimable_WritesCorrectKeyValue()
        {
            var inputHash = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedKey = inputHash.BuildIndexClaimableKey();
            var inputReferences = new List<CoinReference>
            {
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = (ushort) RandomInt(10)
                },
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = (ushort) RandomInt(10)
                }
            };
            var inputSet = inputReferences.ToHashSet();
            var expectedBytes = new byte[1];
            var serializerMock = AutoMockContainer.GetMock<IBinarySerializer>();
            serializerMock
                .Setup(m => m.Serialize(It.Is<CoinReference[]>(arr => arr.SequenceEqual(inputReferences)), null))
                .Returns(expectedBytes);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.SetIndexClaimable(inputHash, inputSet);

            rocksDbContextMock.Verify(m => m.Save(expectedKey, expectedBytes));
        }
    }
}