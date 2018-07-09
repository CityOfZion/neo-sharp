using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
