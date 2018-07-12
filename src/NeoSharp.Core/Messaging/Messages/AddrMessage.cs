using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Converters;

namespace NeoSharp.Core.Messaging.Messages
{
    public class AddrMessage : Message<AddrPayload>
    {
        public AddrMessage()
        {
            Command = MessageCommand.addr;
            Payload = new AddrPayload();
        }

        public AddrMessage(IEnumerable<NetworkAddressWithTime> address)
        {
            Command = MessageCommand.addr;
            Payload = new AddrPayload { Address = address.ToArray() };
        }
    }

    public class AddrPayload
    {
        [BinaryProperty(0)]
        public NetworkAddressWithTime[] Address;
    }

    public class NetworkAddressWithTime : IEquatable<NetworkAddressWithTime>
    {
        [BinaryProperty(0)]
        public uint Timestamp;

        [BinaryProperty(1)]
        public ulong Services;

        [BinaryProperty(2)]
        [TypeConverter(typeof(IpEndPointConverter))]
        [BinaryTypeSerializer(typeof(IpEndPointConverter))]
        public IPEndPoint EndPoint;

        /// <summary>
        /// Check if is equal to other
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if equal</returns>
        public bool Equals(NetworkAddressWithTime other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (other is null) return false;

            return Timestamp == other.Timestamp && Services == other.Services && EndPoint.Equals(other.EndPoint);
        }

        /// <summary>
        /// Check if is equal to other
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if equal</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null) return false;

            if (!(obj is NetworkAddressWithTime other)) return false;

            return Timestamp == other.Timestamp && Services == other.Services && EndPoint.Equals(other.EndPoint);
        }

        /// <summary>
        /// Get HashCode
        /// </summary>
        /// <returns>Return hashcode</returns>
        public override int GetHashCode()
        {
            return EndPoint != null ? EndPoint.ToString().GetHashCode() : 0;
        }
    }
}