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
using NeoSharp.Persistence.RedisDB;
using NeoSharp.Persistence.RedisDB.Helpers;
using NeoSharp.TestHelpers;
using NeoSharp.Types;
using StackExchange.Redis;

namespace NeoSharp.Persistence.Redis.Tests
{
    [TestClass]
    public class UtRedisDbBinaryRepository : TestBase
    {
        private Mock<IBinarySerializer> _serializerMock;

        [TestInitialize]
        public void TestInit()
        {
            _serializerMock = AutoMockContainer.GetMock<IBinarySerializer>();
        }

        [TestMethod]
        public void Ctor_CreateValidRedisDbRepository()
        {
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            testee.Should().BeOfType<RedisDbBinaryRepository>();
        }

        #region IRepository System Members

        [TestMethod]
        public async Task GetTotalBlockHeight_NoValueFound_ReturnsUIntMinValue()
        {
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(x => x.Get(DataEntryPrefix.SysCurrentBlock.ToString()))
                .ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetTotalBlockHeight();

            result.Should().Be(uint.MinValue);
        }

        [TestMethod]
        public async Task GetTotalBlockHeight_ValueFound_ReturnsValue()
        {
            var expectedResult = (uint)RandomInt();
            var expectedValue = (RedisValue) expectedResult;
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(x => x.Get(DataEntryPrefix.SysCurrentBlock.ToString()))
                .ReturnsAsync(expectedValue);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetTotalBlockHeight();

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task SetTotalBlockHeight_SaveCorrectKeyValue()
        {
            var input = (uint)RandomInt();
            var expectedValue = (RedisValue) input;
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.SetTotalBlockHeight(input);

            redisDbContextMock.Verify(m => m.Set(DataEntryPrefix.SysCurrentBlock.ToString(), expectedValue));
        }

        [TestMethod]
        public async Task GetVersion_NoValueFound_ReturnsUIntMinValue()
        {
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(x => x.Get(DataEntryPrefix.SysVersion.ToString()))
                .ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetVersion();

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetVersion_ValueFound_ReturnsValue()
        {
            var expectedResult = RandomString(RandomInt(10));

            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(x => x.Get(DataEntryPrefix.SysVersion.ToString()))
                .ReturnsAsync(expectedResult);

            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetVersion();

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task SetVersion_SaveCorrectKeyValue()
        {
            var input = RandomString(RandomInt(10));

            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.SetVersion(input);

            redisDbContextMock.Verify(m => m.Set(DataEntryPrefix.SysVersion.ToString(), input));
        }

        [TestMethod]
        public async Task GetTotalBlockHeaderHeight_NoValueFound_ReturnsUIntMinValue()
        {
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(x => x.Get(DataEntryPrefix.SysCurrentHeader.ToString()))
                .ReturnsAsync((RedisValue.Null));
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetTotalBlockHeaderHeight();

            result.Should().Be(uint.MinValue);
        }

        [TestMethod]
        public async Task GetTotalBlockHeaderHeight_ValueFound_ReturnsValue()
        {
            var expectedResult = (uint)RandomInt();
            var expectedValue = (RedisValue) expectedResult;
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(x => x.Get(DataEntryPrefix.SysCurrentHeader.ToString()))
                .ReturnsAsync(expectedValue);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetTotalBlockHeaderHeight();

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task SetTotalBlockHeaderHeight_SaveCorrectKeyValue()
        {
            var input = (uint)RandomInt();
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.SetTotalBlockHeaderHeight(input);

            redisDbContextMock.Verify(m => m.Set(DataEntryPrefix.SysCurrentHeader.ToString(), input));
        }

        #endregion

        #region IRepository Data Members

        [TestMethod]
        public async Task GetBlockHashFromHeight_NoHashFound_ReturnUInt256Zero()
        {
            // Arrange
            const uint heightParameter = 0;

            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(x => x.Get(heightParameter.BuildIxHeightToHashKey()))
                .ReturnsAsync(RedisValue.Null);

            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

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

            var expectedUint256 = UInt256.Parse("0x4520462a8c80056291f871da523bff0eb17e29d44ab4317e69ff7a42083cb39d");

            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(x => x.GetFromHashIndex(It.Is<RedisIndex>(a => a == RedisIndex.BlockHeight), It.IsAny<double>()))
                .ReturnsAsync(expectedUint256);

            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

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

            const uint index = 0;

            var timestamp = (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            var blockHeaderParameter = new BlockHeader
            {
                Hash = hash,
                Index = index
                    
            };

            blockHeaderParameter.Timestamp = timestamp;

            _serializerMock
                .Setup(x => x.Serialize(blockHeaderParameter, null))
                .Returns(expectedBlockByteArray);

            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            // Act
            await testee.AddBlockHeader(blockHeaderParameter);

            // Assert
            redisDbContextMock.Verify(x => x.Set(
                    It.IsAny<RedisKey>(),
                    It.IsAny<RedisValue>()),
                Times.Once);
            
            redisDbContextMock.Verify(x => x.AddToIndex(
                    It.Is<RedisIndex>(a => a == RedisIndex.BlockTimestamp),
                    It.Is<UInt256>(a => a == blockHeaderParameter.Hash),
                    It.Is<double>(a => a == blockHeaderParameter.Timestamp)),
                Times.Once);

            redisDbContextMock.Verify(x => x.AddToIndex(
                    It.Is<RedisIndex>(a => a == RedisIndex.BlockHeight),
                    It.Is<UInt256>(a => a == blockHeaderParameter.Hash),
                    It.Is<double>(a => a == blockHeaderParameter.Index)),
                Times.Once);
        }

        #endregion

        #region IRepository State Members

        [TestMethod]
        public async Task GetAccount_NoValue_ReturnsNull()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<string>(b => b == input.BuildStateAccountKey())))
                .ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetAccount(input);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAccount_ValueFound_ReturnsAccount()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedBytes = new byte[1];
            var expectedResult = new Account();
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateAccountKey())))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<Account>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

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
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();
            await testee.AddAccount(input);

            redisDbContextMock.Verify(m =>
                  m.Set(It.Is<RedisKey>(b => b == input.ScriptHash.BuildStateAccountKey()), expectedBytes));
        }

        [TestMethod]
        public async Task DeleteAccount_DeletesCorrectKey()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.DeleteAccount(input);

            redisDbContextMock.Verify(m => m.Delete(It.Is<RedisKey>(b => b == input.BuildStateAccountKey())));
        }

        [TestMethod]
        public async Task GetCoinStates_NoValue_ReturnsNull()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateCoinKey())))
                .ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetCoinStates(input);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetCoinStates_ValueFound_ReturnsCoinStates()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var expectedBytes = new byte[1];
            var expectedResult = new[] { new CoinState() };
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateCoinKey())))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<CoinState[]>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetCoinStates(input);

            result.Should().Equal(expectedResult);
        }

        [TestMethod]
        public async Task AddCoinStates_WritesCorrectKeyValue()
        {
            var inputHash = UInt256.Parse(RandomInt().ToString("X64"));
            var inputStates = new[] { new CoinState() };
            var expectedBytes = new byte[1];
            _serializerMock.Setup(m => m.Serialize(inputStates, null)).Returns(expectedBytes);
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();
            await testee.AddCoinStates(inputHash, inputStates);

            redisDbContextMock.Verify(m =>
                      m.Set(It.Is<RedisKey>(b => b == inputHash.BuildStateCoinKey()), expectedBytes));
        }

        [TestMethod]
        public async Task DeleteCoinStates_DeletesCorrectKey()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.DeleteCoinStates(input);

            redisDbContextMock.Verify(m => m.Delete(It.Is<RedisKey>(b => b == input.BuildStateCoinKey())));
        }

        [TestMethod]
        public async Task GetValidator_NoValue_ReturnsNull()
        {
            var pubkey = new byte[33];
            pubkey[0] = 0x02;
            var input = new ECPoint(pubkey);
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateValidatorKey())))
                .ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

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
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateValidatorKey())))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<Validator>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetValidator(input);

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task AddValidator_WritesCorrectKeyValue()
        {
            var pubkey = new byte[33];
            pubkey[0] = 0x02;
            var point = new ECPoint(pubkey);
            var input = new Validator { PublicKey = point };
            var expectedBytes = new byte[1];
            _serializerMock.Setup(m => m.Serialize(input, null)).Returns(expectedBytes);
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();
            await testee.AddValidator(input);

            redisDbContextMock.Verify(m =>
                m.Set(It.Is<RedisKey>(b => b == point.BuildStateValidatorKey()), expectedBytes));
        }

        [TestMethod]
        public async Task DeleteValidator_DeletesCorrectKey()
        {
            var pubkey = new byte[33];
            pubkey[0] = 0x02;
            var input = new ECPoint(pubkey);
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.DeleteValidator(input);

            redisDbContextMock.Verify(
                m => m.Delete(It.Is<RedisKey>(b => b == input.BuildStateValidatorKey())));
        }

        [TestMethod]
        public async Task GetAsset_NoValue_ReturnsNull()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateAssetKey())))
                .ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetAsset(input);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetAsset_ValueFound_ReturnsValidator()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var expectedBytes = new byte[1];
            var expectedResult = new Asset();
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateAssetKey())))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<Asset>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetAsset(input);

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task AddAsset_WritesCorrectKeyValue()
        {
            var inputHash = UInt256.Parse(RandomInt().ToString("X64"));
            var input = new Asset { Id = inputHash };
            var expectedBytes = new byte[1];
            _serializerMock.Setup(m => m.Serialize(input, null)).Returns(expectedBytes);
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();
            await testee.AddAsset(input);

            redisDbContextMock.Verify(m =>
                m.Set(It.Is<RedisKey>(b => b == inputHash.BuildStateAssetKey()), expectedBytes));
        }

        [TestMethod]
        public async Task DeleteAsset_DeletesCorrectKey()
        {
            var input = UInt256.Parse(RandomInt().ToString("X64"));
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.DeleteAsset(input);

            redisDbContextMock.Verify(
                m => m.Delete(It.Is<RedisKey>(b => b == input.BuildStateAssetKey())));
        }

        [TestMethod]
        public async Task GetContract_NoValue_ReturnsNull()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateContractKey())))
                .ReturnsAsync((byte[])null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetContract(input);

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetContract_ValueFound_ReturnsContract()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedBytes = new byte[1];
            var expectedResult = new Contract();
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateContractKey())))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<Contract>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetContract(input);

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task AddContract_WritesCorrectKeyValue()
        {
            var inputHash = UInt160.Parse(RandomInt().ToString("X40"));
            var input = new Contract { Code = new Code { ScriptHash = inputHash } };
            var expectedBytes = new byte[1];
            _serializerMock.Setup(m => m.Serialize(input, null)).Returns(expectedBytes);
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();
            await testee.AddContract(input);

            redisDbContextMock.Verify(m =>
                m.Set(It.Is<RedisKey>(b => b == inputHash.BuildStateContractKey()), expectedBytes));
        }

        [TestMethod]
        public async Task DeleteContract_DeletesCorrectKey()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.DeleteContract(input);

            redisDbContextMock.Verify(m =>
                m.Delete(It.Is<RedisKey>(b => b == input.BuildStateContractKey())));
        }

        [TestMethod]
        public async Task GetStorage_NoValue_ReturnsNull()
        {
            var input = new StorageKey
            {
                ScriptHash = UInt160.Parse(RandomInt().ToString("X40")),
                Key = new byte[1]
            };
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateStorageKey())))
                .ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

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
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildStateStorageKey())))
                .ReturnsAsync(expectedBytes);
            _serializerMock.Setup(m => m.Deserialize<StorageValue>(expectedBytes, null)).Returns(expectedResult);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetStorage(input);

            result.Should().Be(expectedResult);
        }

        [TestMethod]
        public async Task AddStorage_WritesCorrectKeyValue()
        {
            var inputHash = UInt160.Parse(RandomInt().ToString("X40"));
            var inputKey = new StorageKey { ScriptHash = inputHash, Key = new byte[0] };
            var inputValue = new StorageValue { Value = new byte[1] };
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();
            await testee.AddStorage(inputKey, inputValue);

            redisDbContextMock.Verify(m =>
                m.Set(It.Is<RedisKey>(b => b == inputKey.BuildStateStorageKey()), inputValue.Value));
        }

        [TestMethod]
        public async Task DeleteStorage_DeletesCorrectKey()
        {
            var inputHash = UInt160.Parse(RandomInt().ToString("X40"));
            var input = new StorageKey { ScriptHash = inputHash, Key = new byte[0] };
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.DeleteStorage(input);

            redisDbContextMock.Verify(m => m.Delete(It.Is<RedisKey>(b => b == input.BuildStateStorageKey())));
        }

        #endregion


        #region IRepository Index Members

        [TestMethod]
        public async Task GetIndexHeight_NoValueFound_ReturnsUIntMinValue()
        {
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == DataEntryPrefix.IxIndexHeight.ToString())))
                .ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetIndexHeight();

            result
                .Should()
                .Be(uint.MinValue);
        }

        [TestMethod]
        public async Task GetIndexHeight_ValueFound_ReturnsUint()
        {
            var expectedHeight = (uint)RandomInt();
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == DataEntryPrefix.IxIndexHeight.ToString())))
                .ReturnsAsync(expectedHeight);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetIndexHeight();

            result.Should().Be(expectedHeight);
        }

        [TestMethod]
        public async Task GetIndexConfirmed_NoValueFound_ReturnsEmptySet()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var contextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            contextMock.Setup(m => m.Get(input.BuildIxConfirmedKey())).ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetIndexConfirmed(input);

            result.Should().BeOfType(typeof(HashSet<CoinReference>));
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

            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildIxConfirmedKey())))
                .ReturnsAsync(expectedBytes);
            _serializerMock
                .Setup(m => m.Deserialize<HashSet<CoinReference>>(expectedBytes, null))
                .Returns(expectedReferences);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetIndexConfirmed(input);

            result.Should().BeOfType<HashSet<CoinReference>>();
            result.Count.Should().Be(expectedReferences.Count);
            result.SetEquals(expectedReferences).Should().BeTrue();
        }

        [TestMethod]
        public async Task SetIndexConfirmed_WritesCorrectKeyValue()
        {
            var inputHash = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedKey = inputHash.BuildIxConfirmedKey();
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
                .Setup(m => m.Serialize(It.Is<CoinReference[]>(arr => arr.SequenceEqual(inputReferences.ToArray())), null))
                .Returns(expectedBytes);
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.SetIndexConfirmed(inputHash, inputReferences);

            redisDbContextMock.Verify(m => m.Set(It.Is<RedisKey>(b => b.Equals(expectedKey)), It.Is<RedisValue>(b => b.Equals(expectedBytes))));
        }

        [TestMethod]
        public async Task GetIndexClaimable_NoValueFound_ReturnsEmptySet()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var contextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            contextMock.Setup(m => m.Get(input.BuildIxClaimableKey())).ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetIndexClaimable(input);

            result.Should().BeOfType(typeof(HashSet<CoinReference>));
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

            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            redisDbContextMock
                .Setup(m => m.Get(It.Is<RedisKey>(b => b == input.BuildIxClaimableKey())))
                .ReturnsAsync(expectedBytes);
            _serializerMock
                .Setup(m => m.Deserialize<HashSet<CoinReference>>(expectedBytes, null))
                .Returns(expectedReferences);
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            var result = await testee.GetIndexClaimable(input);

            result.Should().BeOfType<HashSet<CoinReference>>();
            result.Count.Should().Be(expectedReferences.Count);
            result.SetEquals(expectedReferences).Should().BeTrue();
        }

        [TestMethod]
        public async Task SetIndexClaimable_WritesCorrectKeyValue()
        {
            var inputHash = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedKey = inputHash.BuildIxClaimableKey();
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
                .Setup(m => m.Serialize(It.Is<CoinReference[]>(arr => arr.SequenceEqual(inputReferences.ToArray())), null))
                .Returns(expectedBytes);
            var redisDbContextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            var testee = AutoMockContainer.Create<RedisDbBinaryRepository>();

            await testee.SetIndexClaimable(inputHash, inputReferences);

            redisDbContextMock.Verify(m => m.Set(It.Is<RedisKey>(b => b.Equals(expectedKey)), It.Is<RedisValue>(b => b.Equals(expectedBytes))));
        }

        #endregion
    }
}