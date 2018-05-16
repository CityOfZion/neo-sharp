namespace NeoSharp.Core.Network
{
    public class NetworkACLConfig
    {
        /// <summary>
        /// ACL Types
        /// </summary>
        public enum ACLType
        {
            /// <summary>
            /// None ACL
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
        /// ACL behaviour
        /// </summary>
        public ACLType Type { get; internal set; } = ACLType.None;
        /// <summary>
        /// Path of rules file
        /// </summary>
        public string Path { get; internal set; }
    }
}