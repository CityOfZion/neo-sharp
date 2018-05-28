namespace NeoSharp.Core.Network.Security
{
    public class NetworkAclConfig
    {
        /// <summary>
        /// Acl Types
        /// </summary>
        public enum AclType
        {
            /// <summary>
            /// None Acl
            /// </summary>
            None,
            /// <summary>
            /// If match deny
            /// </summary>
            Whitelist,
            /// <summary>
            /// If match allow
            /// </summary>
            Blacklist
        };

        /// <summary>
        /// Acl behaviour
        /// </summary>
        public AclType Type { get; set; } = AclType.None;
        /// <summary>
        /// Path of rules file
        /// </summary>
        public string Path { get; set; }
    }
}