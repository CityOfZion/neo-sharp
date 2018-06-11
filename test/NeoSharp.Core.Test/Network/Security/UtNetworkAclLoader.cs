using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Network.Security;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network.Security
{
    [TestClass]
    public class UtNetworkAclLoader : TestBase
    {
        [TestMethod]
        public void Load_LoadNewInstanceOfNetworkAcl()
        {
            var testee = this.AutoMockContainer.Create<NetworkAclLoader>();

            var result = testee.Load(new NetworkAclConfig());

            result
                .Should()
                .BeAssignableTo<NetworkAcl>();
        }

    }
}
