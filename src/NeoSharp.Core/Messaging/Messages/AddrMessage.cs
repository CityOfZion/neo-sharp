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

    public class NetworkAddressWithTime
    {
        [BinaryProperty(0)]
        public uint Timestamp;

        [BinaryProperty(1)]
        public ulong Services;

        [BinaryProperty(2)]
        [TypeConverter(typeof(IPEndPointConverter))]
        public IPEndPoint EndPoint;
    }
}