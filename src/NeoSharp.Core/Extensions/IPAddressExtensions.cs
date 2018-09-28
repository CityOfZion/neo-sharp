using System.Net;

namespace NeoSharp.Core.Extensions
{
    public static class IpAddressExtensions
    {
        internal static IPAddress Unmap(this IPAddress address)
        {
            if (address.IsIPv4MappedToIPv6)
                address = address.MapToIPv4();
            return address;
        }

        internal static IPEndPoint Unmap(this IPEndPoint endPoint)
        {
            if (!endPoint.Address.IsIPv4MappedToIPv6)
                return endPoint;
            return new IPEndPoint(endPoint.Address.Unmap(), endPoint.Port);
        }
    }
}