namespace NeoSharp.Core.Network.Security
{
    /// <summary>
    /// Acl Types
    /// </summary>
    public enum NetworkAclType
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
}