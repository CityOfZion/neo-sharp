using System.Net;

namespace NeoSharp.Core.Network.Security
{
    public interface INetworkAcl
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
        /// Initiate the Acl
        /// </summary>
        /// <param name="cfg">Config</param>
        void Load(NetworkAclConfig cfg);
    }
}