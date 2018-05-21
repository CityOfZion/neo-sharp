using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeoSharp.Core.Network;
using NeoSharp.TestHelpers;
using System;
using System.IO;

namespace NeoSharp.Core.Test.Network
{
    [TestClass]
    public class UtNetworkACL : TestBase
    {
        [TestMethod]
        public void IsAllowed_Whitelist()
        {
            // Arrange
            var aclFactory = AutoMockContainer.Create<NetworkACLFactory>();
            var acl = aclFactory.CreateNew();
            NetworkACLConfig cfg = new NetworkACLConfig();
            string tmpRules = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");

            File.WriteAllText(tmpRules, @"[{'value':'192\\.168\\.6\\..*','regex':true},{'value':'192.168.5.1','regex':false}]");
            cfg.Path = tmpRules;
            cfg.Type = NetworkACLConfig.ACLType.Whitelist;

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
            var aclFactory = AutoMockContainer.Create<NetworkACLFactory>();
            var acl = aclFactory.CreateNew();
            NetworkACLConfig cfg = new NetworkACLConfig();
            string tmpRules = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");

            File.WriteAllText(tmpRules, @"[{'value':'192\\.168\\.8\\..*','regex':true},{'value':'192.168.7.1','regex':false}]");
            cfg.Path = tmpRules;
            cfg.Type = NetworkACLConfig.ACLType.Blacklist;

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
