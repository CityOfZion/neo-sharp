using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using NeoSharp.Core.Network;
using NeoSharp.Core.Network.Security;

namespace NeoSharp.Core.Test.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        public static NetworkConfig GetNetworkConfig(this string[] peerEndPoints)
        {
            var initialData = new Dictionary<string, string>
            {
                { "network:port", "8000" },
                { "network:forceIPv6", "false" },
            };

            for (var i = 0; i < peerEndPoints.Length; i++)
            {
                initialData.Add($"network:peerEndPoints:{i}", peerEndPoints[i]);
            }

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData)
                .Build();

            return new NetworkConfig(configuration, new NetworkAclLoader());
        }
    }
}
