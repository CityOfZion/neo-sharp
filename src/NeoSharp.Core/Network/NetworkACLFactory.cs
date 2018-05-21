namespace NeoSharp.Core.Network
{
    public class NetworkACLFactory 
    {
        /// <summary>
        /// Create a new ACL
        /// </summary>
        public INetworkACL CreateNew()
        {
            return new NetworkACL();
        }
    }
}