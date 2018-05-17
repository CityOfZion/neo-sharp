using NeoSharp.Core.Network.Tcp;
using NeoSharp.Core.Types.Json;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NeoSharp.Core.Network
{
    public class NetworkACL : INetworkACL
    {
        public class Entry
        {
            /// <summary>
            /// String of the rule
            /// </summary>
            public readonly string Value;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="value">Value</param>
            public Entry(string value) { Value = value; }

            /// <summary>
            /// Macth Value
            /// </summary>
            /// <param name="address">Address</param>
            /// <returns>Return true if match</returns>
            public virtual bool Match(string address)
            {
                return Value == address;
            }
            /// <summary>
            /// String representation
            /// </summary>
            public override string ToString()
            {
                return Value.ToString();
            }
        }

        public class RegexEntry : Entry
        {
            private readonly Regex _cache;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="value">Value</param>
            public RegexEntry(string value) : base(value)
            {
                _cache = new Regex(value);
            }

            /// <summary>
            /// Macth Value
            /// </summary>
            /// <param name="address">Address</param>
            /// <returns>Return true if match</returns>
            public override bool Match(string address)
            {
                return _cache.IsMatch(address);
            }
        }

        /// <summary>
        /// File Entries
        /// </summary>
        public Entry[] Entries { get; private set; }
        /// <summary>
        /// ACL behaviour
        /// </summary>
        public NetworkACLConfig.ACLType Type { get; private set; } = NetworkACLConfig.ACLType.None;

        /// <summary>
        /// Allow or deny string Adresses based on rules
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>True if pass</returns>
        public bool IsAllowed(string address)
        {
            if (string.IsNullOrEmpty(address)) return false;

            switch (Type)
            {
                case NetworkACLConfig.ACLType.Blacklist:
                    {
                        foreach (Entry entry in Entries)
                        {
                            if (entry.Match(address))
                                return false;
                        }
                        return true;
                    }
                case NetworkACLConfig.ACLType.Whitelist:
                    {
                        foreach (Entry entry in Entries)
                        {
                            if (entry.Match(address))
                                return true;
                        }
                        return false;
                    }
                default: return true;
            }
        }
        /// <summary>
        /// Allow or deny IP Adresses based on rules
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>True if pass</returns>
        public bool IsAllowed(IPAddress address)
        {
            return IsAllowed(address?.ToString());
        }
        /// <summary>
        /// Allow or deny Peer based on rules
        /// </summary>
        /// <param name="peer">Peer to check</param>
        /// <returns>True if pass</returns>
        public bool IsAllowed(IPeer peer)
        {
            if (peer is TcpPeer tcp)
            {
                return IsAllowed(tcp.IPAddress);
            }
            else
            {
                // TODO: Remove this line when fix mock
                if (peer.EndPoint == null) return true;

                return IsAllowed(peer?.EndPoint.Host);
            }
        }
        /// <summary>
        /// Initiate the ACL
        /// </summary>
        /// <param name="cfg">Config</param>
        public void Load(NetworkACLConfig cfg)
        {
            if (cfg == null) return;

            Type = cfg.Type;

            if (!string.IsNullOrEmpty(cfg.Path) && File.Exists(cfg.Path))
            {
                string json = File.ReadAllText(cfg.Path);
                JObject jo = JObject.Parse(json);

                if (!(jo is JArray array)) return;

                Entries = new Entry[array.Count];

                for (int x = 0, m = array.Count; x < m; x++)
                {
                    JObject j = array[x];
                    if (!j.ContainsProperty("value")) continue;

                    string value = j.Properties["value"].AsString();

                    if (j.ContainsProperty("regex") &&  j.Properties["regex"].AsBooleanOrDefault(false))
                        Entries[x] = new RegexEntry(value);
                    else
                        Entries[x] = new Entry(value);
                }
            }
        }
    }
}