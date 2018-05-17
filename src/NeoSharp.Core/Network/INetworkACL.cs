using System.Net;

namespace NeoSharp.Core.Network
{
    public interface INetworkACL
    {
        /// <summary>
        /// Allow or denay IP Adresses based on rules
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>True if pass</returns>
        bool IsAllowed(IPAddress address);
        /// <summary>
        /// Allow or denay string Adresses based on rules
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>True if pass</returns>
        bool IsAllowed(string address);
        /// <summary>
        /// Initiate the ACL
        /// </summary>
        /// <param name="cfg">Config</param>
        void Load(NetworkACLConfig cfg);
    }
}