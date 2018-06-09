using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Network.Security;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network.Security
{
    [TestClass]
    public class UtNetworkAclFactory : TestBase
    {
        [TestMethod]
        public void CreateNew_CreateNewInstanceOfINetworkAcl()
        {
            var testee = this.AutoMockContainer.Create<NetworkAclFactory>();

            var result = testee.CreateNew();

            result
                .Should()
                .BeAssignableTo<INetworkAcl>();
        }

    }
}
