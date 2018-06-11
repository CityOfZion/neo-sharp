using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using NeoSharp.Core.Network.Tcp;

namespace NeoSharp.Core.Network.Security
{
    public class NetworkAcl
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
                return string.Equals(Value, address, StringComparison.OrdinalIgnoreCase);
            }

            /// <summary>
            /// String representation
            /// </summary>
            public override string ToString()
            {
                return Value;
            }
        }

        public class RegexEntry : Entry
        {
            private readonly Regex _pattern;

            /// <inheritdoc />
            public RegexEntry(string pattern) : base(pattern)
            {
                _pattern = new Regex(pattern);
            }

            /// <inheritdoc />
            public override bool Match(string address)
            {
                return _pattern.IsMatch(address);
            }
        }

        public NetworkAcl(NetworkAclType type, IReadOnlyCollection<Entry> entries)
        {
            Type = type;
            Entries = entries;
        }

        public static NetworkAcl Default => new NetworkAcl(NetworkAclType.None, new Entry[0]);

        /// <summary>
        /// Acl behaviour
        /// </summary>
        public NetworkAclType Type { get; }

        /// <summary>
        /// Entries
        /// </summary>
        public IReadOnlyCollection<Entry> Entries { get; }

        /// <summary>
        /// Allow or deny IP Adresses based on rules
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>True if pass</returns>
        public bool IsAllowed(string address)
        {
            if (string.IsNullOrEmpty(address)) return false;

            switch (Type)
            {
                case NetworkAclType.Blacklist:
                {
                    return Entries.All(entry => entry.Match(address) == false);
                }

                case NetworkAclType.Whitelist:
                {
                    return Entries.Any(entry => entry.Match(address));
                }

                default: return true;
            }
        }

        /// <summary>
        /// Allow or deny string Adresses based on rules
        /// </summary>
        /// <param name="address">Address to check</param>
        /// <returns>True if pass</returns>
        public bool IsAllowed(IPAddress address)
        {
            return IsAllowed(address?.ToString());
        }

        /// <summary>
        /// Allow or deny end points based on rules
        /// </summary>
        /// <param name="endPoint">Endpoint to check</param>
        /// <returns>True if pass</returns>
        public bool IsAllowed(EndPoint endPoint)
        {
            return IsAllowed(endPoint.Host);
        }
    }
}