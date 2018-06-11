namespace NeoSharp.Core.Network.Security
{
    public class NetworkAclConfig
    {
        /// <summary>
        /// Acl behaviour
        /// </summary>
        public NetworkAclType Type { get; set; } = NetworkAclType.None;

        /// <summary>
        /// Path of rules file
        /// </summary>
        public string Path { get; set; }
    }
}