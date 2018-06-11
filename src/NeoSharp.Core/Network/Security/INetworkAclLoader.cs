namespace NeoSharp.Core.Network.Security
{
    public interface INetworkAclLoader
    {
        /// <summary>
        /// Initiate the Acl
        /// </summary>
        /// <param name="config">Config</param>
        NetworkAcl Load(NetworkAclConfig config);
    }
}