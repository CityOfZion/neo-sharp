using NeoSharp.Core.Network.Tcp;
using System.Net;

namespace NeoSharp.Core.Network
{
    public interface INetworkACL
    {
        /// <summary>
        /// Allow or deny IP Adresses based on rules
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>True if pass</returns>
        bool IsAllowed(IPAddress address);
        /// <summary>
        /// Allow or deny string Adresses based on rules
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>True if pass</returns>
        bool IsAllowed(string address);
        /// <summary>
        /// Allow or deny Peer based on rules
        /// </summary>
        /// <param name="peer">Peer to check</param>
        /// <returns>True if pass</returns>
        bool IsAllowed(IPeer peer);
        /// <summary>
        /// Initiate the ACL
        /// </summary>
        /// <param name="cfg">Config</param>
        void Load(NetworkACLConfig cfg);
    }
}