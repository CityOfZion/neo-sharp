using System;
using System.IO;
using System.Linq;
using System.Net;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.SerializationHooks;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Converters
{
    class IPEndPointConverter : IBinaryCustomSerializable
    {
        public object Deserialize(IBinaryDeserializer deserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            byte[] data = reader.ReadBytes(16);
            if (data.Length != 16) throw new FormatException();

            IPAddress address = new IPAddress(data);
            data = reader.ReadBytes(2);

            if (data.Length != 2) throw new FormatException();
            ushort port = data.Reverse().ToArray().ToUInt16(0);

            return new IPEndPoint(address, port);
        }

        public int Serialize(IBinarySerializer serializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            if (value is IPEndPoint ep)
            {
                writer.Write(ep.Address.MapToIPv6().GetAddressBytes());
                writer.Write(BitConverter.GetBytes((ushort)ep.Port).Reverse().ToArray());

                return 18;
            }

            throw new ArgumentException(nameof(value));
        }
    }
}