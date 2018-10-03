using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Cryptography;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Persistence.RocksDB.Tests
{
    [TestClass]
    public class UtRocksDbRepository : TestBase
    {
        private Mock<IBinarySerializer> _serializerMock;

        [TestInitialize]
        public void TestInit()
        {
            _serializerMock = AutoMockContainer.GetMock<IBinarySerializer>();
        }

        [TestMethod]
        public void Ctor_CreateValidRocksDbRepository()
        {
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            testee.Should().BeOfType<RocksDbRepository>();
        }

        #region IRepository System Members

        [TestMethod]
        public async Task GetTotalBlockHeight_NoValueFound_ReturnsUIntMinValue()
        {
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(It.Is<byte[]>(b => b.SequenceEqual(new[] {(byte) DataEntryPrefix.SysCurrentBlock}))))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetTotalBlockHeight();

            result.Should().Be(uint.MinValue);
        }

        [TestMethod]
        public async Task GetTotalBlockHeight_ValueFound_ReturnsValue()
        {
            var expectedResult = (uint) RandomInt();
            var expectedBytes = BitConverter.GetBytes(expectedResult);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(It.Is<byte[]>(b => b.SequenceEqual(new[] {(byte) DataEntryPrefix.SysCurrentBlock}))))
                .ReturnsAsync(expectedBytes);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetTotalBlockHeight();

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task SetTotalBlockHeight_SaveCorrectKeyValue()
        {
            var input = (uint) RandomInt();
            var expectedValue = BitConverter.GetBytes(input);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.SetTotalBlockHeight(input);

            rocksDbContextMock.Verify(m =>
                m.Save(It.Is<byte[]>(b => b.SequenceEqual(new[] {(byte) DataEntryPrefix.SysCurrentBlock})),
                    It.Is<byte[]>(b => b.SequenceEqual(expectedValue))));
        }

        [TestMethod]
        public async Task GetVersion_NoValueFound_ReturnsUIntMinValue()
        {
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(It.Is<byte[]>(b => b.SequenceEqual(new[] {(byte) DataEntryPrefix.SysVersion}))))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetVersion();

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetVersion_ValueFound_ReturnsValue()
        {
            var expectedResult = RandomString(RandomInt(10));
            var expectedBytes = new byte[1];
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(It.Is<byte[]>(b => b.SequenceEqual(new[] {(byte) DataEntryPrefix.SysVersion}))))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<string>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetVersion();

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task SetVersion_SaveCorrectKeyValue()
        {
            var input = RandomString(RandomInt(10));
            var expectedValue = new byte[1];
            _serializerMock.Setup(m => m.Serialize(input, null)).Returns(expectedValue);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.SetVersion(input);

            rocksDbContextMock.Verify(m =>
                m.Save(It.Is<byte[]>(b => b.SequenceEqual(new[] {(byte) DataEntryPrefix.SysVersion})),
                    expectedValue));
        }

        [TestMethod]
        public async Task GetTotalBlockHeaderHeight_NoValueFound_ReturnsUIntMinValue()
        {
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(It.Is<byte[]>(b => b.SequenceEqual(new[] {(byte) DataEntryPrefix.SysCurrentHeader}))))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetTotalBlockHeaderHeight();

            result.Should().Be(uint.MinValue);
        }

        [TestMethod]
        public async Task GetTotalBlockHeaderHeight_ValueFound_ReturnsValue()
        {
            var expectedResult = (uint) RandomInt();
            var expectedBytes = BitConverter.GetBytes(expectedResult);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(It.Is<byte[]>(b => b.SequenceEqual(new[] {(byte) DataEntryPrefix.SysCurrentHeader}))))
                .ReturnsAsync(expectedBytes);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetTotalBlockHeaderHeight();

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task SetTotalBlockHeaderHeight_SaveCorrectKeyValue()
        {
            var input = (uint) RandomInt();
            var expectedValue = BitConverter.GetBytes(input);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.SetTotalBlockHeaderHeight(input);

            rocksDbContextMock.Verify(m =>
                m.Save(It.Is<byte[]>(b => b.SequenceEqual(new[] {(byte) DataEntryPrefix.SysCurrentHeader})),
                    It.Is<byte[]>(b => b.SequenceEqual(expectedValue))));
        }

        #endregion

        #region IRepository Data Members

        [TestMethod]
        public async Task GetBlockHashFromHeight_NoHashFound_ReturnUInt256Zero()
        {
            // Arrange
            const uint heightParameter = 0;

            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(x => x.Get(heightParameter.BuildIxHeightToHashKey()))
                .ReturnsAsync((byte[]) null);

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
                .ReturnsAsync(byteArrayParameter);

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

            var blockHeaderParameter = new BlockHeader
            {
                Hash = hash,
                Index = index
            };
            _serializerMock
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

        #endregion

        #region IRepository State Members

        [TestMethod]
        public async Task GetAccount_NoValue_ReturnsNull()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateAccountKey()))))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetAccount(input);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAccount_ValueFound_ReturnsAccount()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedBytes = new byte[1];
            var expectedResult = new Account();
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateAccountKey()))))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<Account>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetAccount(input);

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task AddAccount_WritesCorrectKeyValue()
        {
            var input = new Account
            {
                ScriptHash = UInt160.Parse(RandomInt().ToString("X40"))
            };
            var expectedBytes = new byte[1];
            _serializerMock.Setup(m => m.Serialize(input, null)).Returns(expectedBytes);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();
            await testee.AddAccount(input);

            rocksDbContextMock.Verify(m =>
                m.Save(It.Is<byte[]>(b => b.SequenceEqual(input.ScriptHash.BuildStateAccountKey())), expectedBytes));
        }

        [TestMethod]
        public async Task DeleteAccount_DeletesCorrectKey()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.DeleteAccount(input);

            rocksDbContextMock.Verify(m => m.Delete(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateAccountKey()))));
        }

        [TestMethod]
        public async Task GetCoinStates_NoValue_ReturnsNull()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateCoinKey()))))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetCoinStates(input);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetCoinStates_ValueFound_ReturnsCoinStates()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var expectedBytes = new byte[1];
            var expectedResult = new[] {new CoinState()};
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateCoinKey()))))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<CoinState[]>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetCoinStates(input);

            result.Should().Equal(expectedResult);
        }

        [TestMethod]
        public async Task AddCoinStates_WritesCorrectKeyValue()
        {
            var inputHash = UInt256.Parse(RandomInt().ToString("X64"));
            var inputStates = new[] {new CoinState()};
            var expectedBytes = new byte[1];
            _serializerMock.Setup(m => m.Serialize(inputStates, null)).Returns(expectedBytes);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();
            await testee.AddCoinStates(inputHash, inputStates);

            rocksDbContextMock.Verify(m =>
                m.Save(It.Is<byte[]>(b => b.SequenceEqual(inputHash.BuildStateCoinKey())), expectedBytes));
        }

        [TestMethod]
        public async Task DeleteCoinStates_DeletesCorrectKey()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.DeleteCoinStates(input);

            rocksDbContextMock.Verify(m => m.Delete(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateCoinKey()))));
        }

        [TestMethod]
        public async Task GetValidator_NoValue_ReturnsNull()
        {
            var pubkey = new byte[33];
            pubkey[0] = 0x02;
            var input = new ECPoint(pubkey);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateValidatorKey()))))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetValidator(input);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetValidator_ValueFound_ReturnsValidator()
        {
            var pubkey = new byte[33];
            pubkey[0] = 0x02;
            var input = new ECPoint(pubkey);
            var expectedBytes = new byte[1];
            var expectedResult = new Validator();
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateValidatorKey()))))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<Validator>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetValidator(input);

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task AddValidator_WritesCorrectKeyValue()
        {
            var pubkey = new byte[33];
            pubkey[0] = 0x02;
            var point = new ECPoint(pubkey);
            var input = new Validator {PublicKey = point};
            var expectedBytes = new byte[1];
            _serializerMock.Setup(m => m.Serialize(input, null)).Returns(expectedBytes);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();
            await testee.AddValidator(input);

            rocksDbContextMock.Verify(m =>
                m.Save(It.Is<byte[]>(b => b.SequenceEqual(point.BuildStateValidatorKey())), expectedBytes));
        }

        [TestMethod]
        public async Task DeleteValidator_DeletesCorrectKey()
        {
            var pubkey = new byte[33];
            pubkey[0] = 0x02;
            var input = new ECPoint(pubkey);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.DeleteValidator(input);

            rocksDbContextMock.Verify(
                m => m.Delete(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateValidatorKey()))));
        }

        [TestMethod]
        public async Task GetAsset_NoValue_ReturnsNull()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateAssetKey()))))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetAsset(input);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAsset_ValueFound_ReturnsValidator()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var expectedBytes = new byte[1];
            var expectedResult = new Asset();
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateAssetKey()))))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<Asset>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetAsset(input);

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task AddAsset_WritesCorrectKeyValue()
        {
            var inputHash = UInt256.Parse(RandomInt().ToString("X64"));
            var input = new Asset {Id = inputHash};
            var expectedBytes = new byte[1];
            _serializerMock.Setup(m => m.Serialize(input, null)).Returns(expectedBytes);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();
            await testee.AddAsset(input);

            rocksDbContextMock.Verify(m =>
                m.Save(It.Is<byte[]>(b => b.SequenceEqual(inputHash.BuildStateAssetKey())), expectedBytes));
        }

        [TestMethod]
        public async Task DeleteAsset_DeletesCorrectKey()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.DeleteAsset(input);

            rocksDbContextMock.Verify(
                m => m.Delete(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateAssetKey()))));
        }

        [TestMethod]
        public async Task GetContract_NoValue_ReturnsNull()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateContractKey()))))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetContract(input);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetContract_ValueFound_ReturnsContract()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedBytes = new byte[1];
            var expectedResult = new Contract();
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateContractKey()))))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<Contract>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetContract(input);

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task AddContract_WritesCorrectKeyValue()
        {
            var inputHash = UInt160.Parse(RandomInt().ToString("X40"));
            var input = new Contract {Code = new Code {ScriptHash = inputHash}};
            var expectedBytes = new byte[1];
            _serializerMock.Setup(m => m.Serialize(input, null)).Returns(expectedBytes);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();
            await testee.AddContract(input);

            rocksDbContextMock.Verify(m =>
                m.Save(It.Is<byte[]>(b => b.SequenceEqual(inputHash.BuildStateContractKey())), expectedBytes));
        }

        [TestMethod]
        public async Task DeleteContract_DeletesCorrectKey()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.DeleteContract(input);

            rocksDbContextMock.Verify(m =>
                m.Delete(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateContractKey()))));
        }

        [TestMethod]
        public async Task GetStorage_NoValue_ReturnsNull()
        {
            var input = new StorageKey
            {
                ScriptHash = UInt160.Parse(RandomInt().ToString("X40")),
                Key = new byte[1]
            };
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateStorageKey()))))
                .ReturnsAsync((byte[]) null);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetStorage(input);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetStorage_ValueFound_ReturnsStorageValue()
        {
            var input = new StorageKey
            {
                ScriptHash = UInt160.Parse(RandomInt().ToString("X40")),
                Key = new byte[1]
            };
            var expectedBytes = new byte[1];
            var expectedResult = new StorageValue();
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateStorageKey()))))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<StorageValue>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetStorage(input);

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task AddStorage_WritesCorrectKeyValue()
        {
            var inputHash = UInt160.Parse(RandomInt().ToString("X40"));
            var inputKey = new StorageKey {ScriptHash = inputHash, Key = new byte[0]};
            var inputValue = new StorageValue {Value = new byte[1]};
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();
            await testee.AddStorage(inputKey, inputValue);

            rocksDbContextMock.Verify(m =>
                m.Save(It.Is<byte[]>(b => b.SequenceEqual(inputKey.BuildStateStorageKey())), inputValue.Value));
        }

        [TestMethod]
        public async Task DeleteStorage_DeletesCorrectKey()
        {
            var inputHash = UInt160.Parse(RandomInt().ToString("X40"));
            var input = new StorageKey {ScriptHash = inputHash, Key = new byte[0]};
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.DeleteStorage(input);

            rocksDbContextMock.Verify(m => m.Delete(It.Is<byte[]>(b => b.SequenceEqual(input.BuildStateStorageKey()))));
        }

        #endregion

        #region IRepository Index Members

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
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildIndexConfirmedKey()))))
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
            }.ToHashSet();

            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildIndexConfirmedKey()))))
                .ReturnsAsync(expectedBytes);
            _serializerMock
                .Setup(m => m.Deserialize<HashSet<CoinReference>>(expectedBytes, null))
                .Returns(expectedReferences);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetIndexConfirmed(input);

            result.Should().BeOfType<HashSet<CoinReference>>();
            result.Count.Should().Be(expectedReferences.Count);
            result.SetEquals(expectedReferences).Should().BeTrue();
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
            }.ToHashSet();
            var expectedBytes = new byte[1];
            _serializerMock
                .Setup(m => m.Serialize(It.Is<HashSet<CoinReference>>(arr => arr.SetEquals(inputReferences)), null))
                .Returns(expectedBytes);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.SetIndexConfirmed(inputHash, inputReferences);

            rocksDbContextMock.Verify(m => m.Save(It.Is<byte[]>(b => b.SequenceEqual(expectedKey)), expectedBytes));
        }

        [TestMethod]
        public async Task GetIndexClaimable_NoValueFound_ReturnsEmptySet()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildIndexClaimableKey()))))
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
            }.ToHashSet();

            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            rocksDbContextMock
                .Setup(m => m.Get(It.Is<byte[]>(b => b.SequenceEqual(input.BuildIndexClaimableKey()))))
                .ReturnsAsync(expectedBytes);
            _serializerMock
                .Setup(m => m.Deserialize<HashSet<CoinReference>>(expectedBytes, null))
                .Returns(expectedReferences);
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            var result = await testee.GetIndexClaimable(input);

            result.Should().BeOfType<HashSet<CoinReference>>();
            result.Count.Should().Be(expectedReferences.Count);
            result.SetEquals(expectedReferences).Should().BeTrue();
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
            }.ToHashSet();
            var expectedBytes = new byte[1];
            _serializerMock
                .Setup(m => m.Serialize(It.Is<HashSet<CoinReference>>(arr => arr.SetEquals(inputReferences)), null))
                .Returns(expectedBytes);
            var rocksDbContextMock = AutoMockContainer.GetMock<IRocksDbContext>();
            var testee = AutoMockContainer.Create<RocksDbRepository>();

            await testee.SetIndexClaimable(inputHash, inputReferences);

            rocksDbContextMock.Verify(m => m.Save(expectedKey, expectedBytes));
        }

        #endregion
    }
}