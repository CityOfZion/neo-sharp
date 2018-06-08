using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging.Messages;
using System;
using System.Reflection;

namespace NeoSharp.Core.Network
{
    public class ServerContext : IServerContext
    {
        /// <summary>
        /// Version
        /// </summary>
        public VersionPayload Version { get; private set; }

        /// <summary>
        /// Server context
        /// </summary>
        public ServerContext()
        {
            Version = new VersionPayload
            {
                Version = 2,
                // TODO: What's it?
                // Services = NetworkAddressWithTime.NODE_NETWORK;
                Timestamp = DateTime.UtcNow.ToTimestamp(),
                Port = 0,
                Nonce = (uint)new Random(Environment.TickCount).Next(),
                UserAgent = $"/NEO:{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}/",
                CurrentBlockIndex = 0,
                Relay = true
            };
        }

        public void BuiltVersionPayload(ushort port, uint blockchainCurrentBlockIndex)
        {
            Version.Port = port;
            Version.Timestamp = DateTime.UtcNow.ToTimestamp();
            Version.CurrentBlockIndex = blockchainCurrentBlockIndex;
        }

        public void BuiltVersionPayload(uint blockchainCurrentBlockIndex)
        {
            Version.Timestamp = DateTime.UtcNow.ToTimestamp();
            Version.CurrentBlockIndex = blockchainCurrentBlockIndex;
        }
    }
}