using System;
using System.Reflection;
using NeoSharp.Core.Blockchain;
using NeoSharp.Core.Extensions;
using NeoSharp.Core.Messaging.Messages;

namespace NeoSharp.Core.Network
{
    public class ServerContext : IServerContext
    {
        #region Consts

        const uint PROTOCOL_VERSION = 0;
        const ulong NODE_NETWORK = 1;

        #endregion

        #region Variables

        /// <summary>
        /// Blockchain
        /// </summary>
        private readonly IBlockchain _blockchain;
        /// <summary>
        /// Cached version
        /// </summary>
        private readonly VersionPayload _version;

        #endregion

        #region Properties

        /// <inheritdoc />
        public VersionPayload Version
        {
            get
            {
                _version.Timestamp = DateTime.UtcNow.ToTimestamp();
                _version.CurrentBlockIndex = _blockchain.CurrentBlock?.Index ?? 0;

                return _version;
            }
        }

        #endregion

        /// <summary>
        /// Server context
        /// </summary>
        /// <param name="config">Config</param>
        /// <param name="blockchain">Blockchain</param>
        public ServerContext(NetworkConfig config, IBlockchain blockchain)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            _blockchain = blockchain ?? throw new ArgumentNullException(nameof(blockchain));

            _version = new VersionPayload
            {
                Version = PROTOCOL_VERSION,
                Services = NODE_NETWORK,
                Timestamp = DateTime.UtcNow.ToTimestamp(),
                Port = config.Port,
                Nonce = (uint)new Random(Environment.TickCount).Next(),
                UserAgent = $"/NEO-Sharp:{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)}/",
                CurrentBlockIndex = _blockchain.CurrentBlock?.Index ?? 0,
                Relay = true
            };
        }
    }
}
