using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NeoSharp.Core.Blockchain.State;
using NeoSharp.Core.Models;
using NeoSharp.Core.Persistence;
using NeoSharp.TestHelpers;
using NeoSharp.Types;

namespace NeoSharp.Core.Test.Blockchain.State
{
    [TestClass]
    public class UtAccountManager : TestBase
    {
        [TestMethod]
        public async Task Get_ReturnsRepositoryValue()
        {
            var input = UInt160.Parse(RandomInt().ToString("X40"));
            var expectedResult = new Account();
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            repositoryMock.Setup(m => m.GetAccount(input)).ReturnsAsync(expectedResult);
            var testee = AutoMockContainer.Create<AccountManager>();

            var result = await testee.Get(input);
            result.Should().Be(expectedResult);
        }

        [TestMethod]
        [Ignore("Ignored due to GenesisAssets unable to be serialized")]
        public async Task UpdateBalance_NullAccount_InitializesNewAccount()
        {
            var acctHash = UInt160.Parse(RandomInt().ToString("X40"));
            var assetId = UInt256.Parse(RandomInt().ToString("X64"));
            var change = RandomInt();

            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            repositoryMock.Setup(m => m.GetAccount(acctHash)).ReturnsAsync((Account) null);
            var testee = AutoMockContainer.Create<AccountManager>();

            await testee.UpdateBalance(acctHash, assetId, new Fixed8(change));

            repositoryMock.Verify(m => m.AddAccount(It.Is<Account>(a =>
                a.ScriptHash.Equals(acctHash) && a.Balances[assetId].Equals(new Fixed8(change)))));
        }

        [TestMethod]
        [Ignore("Ignored due to GenesisAssets unable to be serialized")]
        public async Task UpdateBalance_ChangesBalanceByDelta()
        {
            var acctHash = UInt160.Parse(RandomInt().ToString("X40"));
            var assetId = UInt256.Parse(RandomInt().ToString("X64"));
            var change = RandomInt(1) == 0 ? RandomInt() : -RandomInt();
            var originalBalance = RandomInt();
            var expectedBalance = originalBalance + change;
            var acct = new Account
            {
                ScriptHash = UInt160.Parse(RandomInt().ToString("X40")),
                Balances = new Dictionary<UInt256, Fixed8>
                {
                    {assetId, new Fixed8(originalBalance)}
                }
            };
            var repositoryMock = AutoMockContainer.GetMock<IRepository>();
            repositoryMock.Setup(m => m.GetAccount(acctHash)).ReturnsAsync(acct);
            var testee = AutoMockContainer.Create<AccountManager>();

            await testee.UpdateBalance(acctHash, assetId, new Fixed8(change));

            repositoryMock.Verify(m => m.AddAccount(It.Is<Account>(a =>
                a.ScriptHash.Equals(acct.ScriptHash) && a.Balances[assetId].Equals(new Fixed8(expectedBalance)))));
        }

    }
}