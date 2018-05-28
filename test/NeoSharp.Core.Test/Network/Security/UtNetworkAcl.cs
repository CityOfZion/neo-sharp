using System;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Network.Security;
using NeoSharp.TestHelpers;

namespace NeoSharp.Core.Test.Network.Security
{
    [TestClass]
    public class UtNetworkAcl : TestBase
    {
        [TestMethod]
        public void IsAllowed_Whitelist()
        {
            // Arrange
            var aclFactory = AutoMockContainer.Create<NetworkAclFactory>();
            var acl = aclFactory.CreateNew();
            var cfg = new NetworkAclConfig();
            var tmpRules = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

            File.WriteAllText(tmpRules, @"[{'value':'192\\.168\\.6\\..*','regex':true},{'value':'192.168.5.1','regex':false}]");
            cfg.Path = tmpRules;
            cfg.Type = NetworkAclConfig.AclType.Whitelist;

            // Act
            acl?.Load(cfg);
            File.Delete(tmpRules);

            // Asset
            for(var i= 0; i< 256; ++i)
            {
                acl.IsAllowed("192.168.6." + i).Should().BeTrue();
                acl.IsAllowed("192.168." + i + ".1").Should().Be(i == 6 || i == 5);
                acl.IsAllowed("192.168.5." + i).Should().Be(i == 1);
            }
        }

        [TestMethod]
        public void IsAllowed_Blacklist()
        {
            // Arrange
            var aclFactory = AutoMockContainer.Create<NetworkAclFactory>();
            var acl = aclFactory.CreateNew();
            var cfg = new NetworkAclConfig();
            var tmpRules = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

            File.WriteAllText(tmpRules, @"[{'value':'192\\.168\\.8\\..*','regex':true},{'value':'192.168.7.1','regex':false}]");
            cfg.Path = tmpRules;
            cfg.Type = NetworkAclConfig.AclType.Blacklist;

            // Act
            acl?.Load(cfg);
            File.Delete(tmpRules);

            // Asset
            for (var i = 0; i < 256; ++i)
            {
                acl.IsAllowed("192.168.8." + i).Should().BeFalse();
                acl.IsAllowed("192.168." + i + ".1").Should().Be(i != 8 && i != 7);
                acl.IsAllowed("192.168.7." + i).Should().Be(i != 1);
            }
        }
    }
}
