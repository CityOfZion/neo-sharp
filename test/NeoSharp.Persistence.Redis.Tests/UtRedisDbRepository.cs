using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.DI;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.Core.Types;
using NeoSharp.Persistence.RedisDB;
using NeoSharp.TestHelpers;
using StackExchange.Redis;

namespace NeoSharp.Persistence.Redis.Tests
{
    [TestClass]
    public class UtRedisDbRepository : TestBase
    {
        [TestMethod]
        public async Task GetIndexHeight_NoValueFound_ReturnsZero()
        {
            var contextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            contextMock.Setup(m => m.Get(DataEntryPrefix.IxIndexHeight.ToString())).ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbRepository>();

            var result = await testee.GetIndexHeight();

            result.Should().Be(uint.MinValue);
        }

        [TestMethod]
        public async Task GetIndexHeight_ValueFound_ReturnsValue()
        {
            var expectedHeight = (uint) RandomInt();
            var contextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            contextMock.Setup(m => m.Get(DataEntryPrefix.IxIndexHeight.ToString())).ReturnsAsync(expectedHeight);
            var testee = AutoMockContainer.Create<RedisDbRepository>();

            var result = await testee.GetIndexHeight();

            result.Should().Be(expectedHeight);
        }

        [TestMethod]
        public async Task GetIndexConfirmed_NoValueFound_ReturnsEmptySet()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var contextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            contextMock.Setup(m => m.Get(input.BuildIxConfirmedKey())).ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbRepository>();

            var result = await testee.GetIndexConfirmed(input);

            result.Should().BeOfType(typeof(HashSet<CoinReference>));
            result.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task GetIndexConfirmed_BinaryValueFound_ReturnsValue()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedBytes = new byte[1];
            var expectedSet = new HashSet<CoinReference>
            {
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = 0
                },
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = 0
                }
            };
            var contextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            contextMock.SetupGet(m => m.IsBinaryMode).Returns(true);
            contextMock.Setup(m => m.Get(input.BuildIxConfirmedKey())).ReturnsAsync(expectedBytes);
            var deserializerMock = AutoMockContainer.GetMock<IBinaryDeserializer>();
            deserializerMock.Setup(m => m.Deserialize<HashSet<CoinReference>>(expectedBytes, null))
                .Returns(expectedSet);
            var testee = AutoMockContainer.Create<RedisDbRepository>();

            var binaryInitializer = new BinaryInitializer(null, deserializerMock.Object);
            var result = await testee.GetIndexConfirmed(input);

            result.SetEquals(expectedSet).Should().BeTrue();
        }

        [TestMethod]
        public async Task GetIndexClaimable_NoValueFound_ReturnsEmptySet()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var contextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            contextMock.Setup(m => m.Get(input.BuildIxClaimableKey())).ReturnsAsync(RedisValue.Null);
            var testee = AutoMockContainer.Create<RedisDbRepository>();

            var result = await testee.GetIndexClaimable(input);

            result.Should().BeOfType(typeof(HashSet<CoinReference>));
            result.Count.Should().Be(0);
        }

        [TestMethod]
        public async Task GetIndexClaimable_BinaryValueFound_ReturnsValue()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedBytes = new byte[1];
            var expectedSet = new HashSet<CoinReference>
            {
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = 0
                },
                new CoinReference
                {
                    PrevHash = UInt256.Parse(RandomInt().ToString("X64")),
                    PrevIndex = 0
                }
            };
            var contextMock = AutoMockContainer.GetMock<IRedisDbContext>();
            contextMock.SetupGet(m => m.IsBinaryMode).Returns(true);
            contextMock.Setup(m => m.Get(input.BuildIxClaimableKey())).ReturnsAsync(expectedBytes);
            var deserializerMock = AutoMockContainer.GetMock<IBinaryDeserializer>();
            deserializerMock.Setup(m => m.Deserialize<HashSet<CoinReference>>(expectedBytes, null))
                .Returns(expectedSet);
            var binaryInitializer = new BinaryInitializer(null, deserializerMock.Object);
            var testee = AutoMockContainer.Create<RedisDbRepository>();

            var result = await testee.GetIndexClaimable(input);

            result.SetEquals(expectedSet).Should().BeTrue();
        }

        //TODO: Test JSON values
    }
}