using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.SerializationHooks;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Converters
{
    class IPEndPointConverter : TypeConverter, IBinaryCustomSerialization
    {
        public static readonly int FixedLength = 18;

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(IPEndPoint) || destinationType == typeof(byte[]) || destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is IPEndPoint ep)
            {
                if (destinationType == typeof(IPEndPoint)) return ep;

                var ip = ep.Address;

                if (destinationType == typeof(byte[]))
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        ip = ip.MapToIPv6();

                    byte[] address = ip.GetAddressBytes();
                    byte[] port = BitConverter.GetBytes((ushort)ep.Port);
                    Array.Reverse(port);

                    return address.Concat(port).ToArray();
                }

                if (destinationType == typeof(string)) return ip.ToString() + "," + ep.Port.ToString();
            }

            if (value == null)
            {
                return null;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(IPEndPoint) || sourceType == typeof(byte[]) || sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is IPEndPoint) return value;
            if (value is byte[] bytes && bytes.Length == FixedLength)
            {
                // Reverse port

                Array.Reverse(bytes, 16, 2);

                IPAddress address = new IPAddress(bytes.Take(16).ToArray());
                ushort port = BitConverter.ToUInt16(bytes, 16);

                return new IPEndPoint(address, port);
            }
            if (value is string str)
            {
                string[] sp = str.Split(',');
                if (sp.Length != 2) throw new FormatException();

                if (!IPAddress.TryParse(sp[0], out var address)) throw new FormatException();
                if (!ushort.TryParse(sp[1], out var port)) throw new FormatException();

                return new IPEndPoint(address, port);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public object Deserialize(IBinaryDeserializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            var bytes = new byte[FixedLength];
            reader.Read(bytes, 0, FixedLength);

            // Reverse port

            IPAddress address = new IPAddress(bytes.Take(16).ToArray());
            ushort port = BitConverter.ToUInt16(bytes, 16);

            return new IPEndPoint(address, port);
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            if (value is IPEndPoint ep)
            {
                var ip = ep.Address;

                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    ip = ip.MapToIPv6();

                byte[] address = ip.GetAddressBytes();
                byte[] port = BitConverter.GetBytes((ushort)ep.Port);
                Array.Reverse(port);

                writer.Write(address, 0, 16);
                writer.Write(port, 0, 2);

                return FixedLength;
            }

            throw new ArgumentException(nameof(value));
        }
    }
}