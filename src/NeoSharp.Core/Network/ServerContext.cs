using System;
using System.Reflection;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Network
{
    public class ServerContext : IServerContext
    {
        public VersionPayload Version { get; private set; }

        public void BuiltVersionPayload(ushort port, uint blockchainCurrentBlockIndex)
        {
            Version = new VersionPayload
            {
                Version = 2,
                Timestamp = DateTime.UtcNow.ToTimestamp(),
                Port = port,
                Nonce = (uint)new Random(Environment.TickCount).Next(),
                UserAgent = $"/NEO:{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}/",
                CurrentBlockIndex = blockchainCurrentBlockIndex,
                Relay = true
            };
        }
    }
}