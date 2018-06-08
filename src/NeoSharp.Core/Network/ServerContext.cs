using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging.Messages;
using System;
using System.Reflection;

namespace NeoSharp.Core.Network
{
    public class ServerContext : IServerContext
    {
        /// <summary>
        /// Blockchain
        /// </summary>
        private readonly IBlockchain _blockchain;

        /// <summary>
        /// Version
        /// </summary>
        public VersionPayload Version { get; private set; }

        /// <summary>
        /// Server context
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="blockchain">Blockchain</param>
        public ServerContext(NetworkConfig config, IBlockchain blockchain)
        {
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));

            Version = new VersionPayload
            {
                Version = 2,
                // TODO: What's it?
                // Services = NetworkAddressWithTime.NODE_NETWORK;
                Timestamp = DateTime.UtcNow.ToTimestamp(),
                Port = config.Port,
                Nonce = (uint)new Random(Environment.TickCount).Next(),
                UserAgent = $"/NEO:{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}/",
                CurrentBlockIndex = 0,
                Relay = true
            };
        }
        /// <summary>
        /// Update version payload
        /// </summary>
        public void UpdateVersionPayload()
        {
            Version.Timestamp = DateTime.UtcNow.ToTimestamp();
            Version.CurrentBlockIndex = _blockchain.CurrentBlock?.Index ?? 0;
        }
    }
}