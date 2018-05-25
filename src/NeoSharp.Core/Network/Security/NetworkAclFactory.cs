namespace NeoSharp.Core.Network.Security
{
    public class NetworkAclFactory 
    {
        /// <summary>
        /// Create a new Acl
        /// </summary>
        public INetworkAcl CreateNew()
        {
            return new NetworkAcl();
        }
    }
}