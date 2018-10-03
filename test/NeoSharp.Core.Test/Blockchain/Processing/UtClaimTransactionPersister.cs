using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.Processing;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Blockchain.Processing
{
    [TestClass]
    public class UtClaimTransactionPersister : TestBase
    {
        [TestMethod]
        public async Task Persist_MarksCorrectCoinStateAsClaimed()
        {
            var txHash = UInt256.Parse(RandomInt().ToString("X64"));
            var expectedClaimIndex = (ushort)RandomInt(3);
            var input = new ClaimTransaction
            {
                Claims = new[]
                {
                    new CoinReference {PrevHash = txHash, PrevIndex = expectedClaimIndex}
                }
            };
            var coinStates = new[]
            {
                CoinState.Confirmed | CoinState.Spent,
                CoinState.Confirmed | CoinState.Spent,
                CoinState.Confirmed | CoinState.Spent
            };
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            repositoryMock.Setup(m => m.GetCoinStates(txHash)).ReturnsAsync(coinStates);
            var testee = AutoMockContainer.Create<ClaimTransactionPersister>();

            await testee.Persist(input);

            repositoryMock.Verify(m => m.AddCoinStates(It.Is<UInt256>(u => u.Equals(txHash)), coinStates));
            for (var i = 0; i < coinStates.Length; i++)
            {
                coinStates[i].Should().HaveFlag(CoinState.Confirmed);
                coinStates[i].Should().HaveFlag(CoinState.Spent);
                if (i == expectedClaimIndex) coinStates[i].Should().HaveFlag(CoinState.Claimed);
            }
        }

        [TestMethod]
        public async Task Persist_MarksClaimsFromMultipleTx()
        {
            var txHash1 = UInt256.Parse(RandomInt().ToString("X64"));
            var txHash2 = UInt256.Parse(RandomInt().ToString("X64"));
            var txHash3 = UInt256.Parse(RandomInt().ToString("X64"));
            var input = new ClaimTransaction
            {
                Claims = new[]
                {
                    new CoinReference {PrevHash = txHash1, PrevIndex = 0},
                    new CoinReference {PrevHash = txHash2, PrevIndex = 0},
                    new CoinReference {PrevHash = txHash3, PrevIndex = 0}
                }
            };
            var coinStates = new[]
            {
                CoinState.Confirmed | CoinState.Spent,
                CoinState.Confirmed | CoinState.Spent,
                CoinState.Confirmed | CoinState.Spent
            };
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            repositoryMock.Setup(m => m.GetCoinStates(txHash1))
                .ReturnsAsync(coinStates.Skip(0).Take(1).ToArray());
            repositoryMock.Setup(m => m.GetCoinStates(txHash2))
                .ReturnsAsync(coinStates.Skip(1).Take(1).ToArray());
            repositoryMock.Setup(m => m.GetCoinStates(txHash3))
                .ReturnsAsync(coinStates.Skip(2).Take(1).ToArray());
            var testee = AutoMockContainer.Create<ClaimTransactionPersister>();

            await testee.Persist(input);

            repositoryMock.Verify(m => m.AddCoinStates(
                It.Is<UInt256>(u => u.Equals(txHash1)),
                It.Is<CoinState[]>(c => c[0].HasFlag(CoinState.Confirmed | CoinState.Spent | CoinState.Claimed))
            ));
            repositoryMock.Verify(m => m.AddCoinStates(
                It.Is<UInt256>(u => u.Equals(txHash2)),
                It.Is<CoinState[]>(c => c[0].HasFlag(CoinState.Confirmed | CoinState.Spent | CoinState.Claimed))
            ));
            repositoryMock.Verify(m => m.AddCoinStates(
                It.Is<UInt256>(u => u.Equals(txHash3)),
                It.Is<CoinState[]>(c => c[0].HasFlag(CoinState.Confirmed | CoinState.Spent | CoinState.Claimed))
            ));
        }
    }
}