using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;

namespace NeoSharp.Core.Network.Rpc
{
    public class RpcConfig
    {
        public class SSLCert
        {
            /// <summary>
            /// Path
            /// </summary>
            public string Path { get; internal set; }
            /// <summary>
            /// Password
            /// </summary>
            public string Password { get; internal set; }

            /// <summary>
            /// Is valid?
            /// </summary>
            public bool IsValid => !string.IsNullOrEmpty(Path) && !string.IsNullOrEmpty(Password) && File.Exists(Path);
        }

        /// <summary>
        /// Listen end point
        /// </summary>
        public IPEndPoint ListenEndPoint { get; internal set; }
        /// <summary>
        /// SSL config
        /// </summary>
        public SSLCert SSL { get; internal set; }
        /// <summary>
        /// ACL Config
        /// </summary>
        public NetworkACLConfig ACL { get; internal set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public RpcConfig(IConfiguration configuration = null)
        {
            configuration?.GetSection("rpc")?.Bind(this);
        }
    }
}