using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.SerializationHooks;
using NeoSharp.Core.Network;

namespace NeoSharp.Core.Converters
{
    class EndPointConverter : TypeConverter, IBinaryCustomSerializable
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(EndPoint) ||
                   destinationType == typeof(byte[]) ||
                   destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is EndPoint ep)
            {
                if (destinationType == typeof(EndPoint)) return ep;

                if (destinationType == typeof(byte[]))
                {
                    return Encoding.UTF8.GetBytes(ep.ToString());
                }

                if (destinationType == typeof(string)) return ep.ToString();
            }

            return value != null ? base.ConvertTo(context, culture, value, destinationType) : null;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(EndPoint) ||
                   sourceType == typeof(byte[]) ||
                   sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is EndPoint) return value;
            if (value is byte[] bytes)
            {
                return EndPoint.Parse(Encoding.UTF8.GetString(bytes));
            }

            if (value is string str)
            {
                return EndPoint.Parse(str);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public object Deserialize(IBinarySerializer binaryDeserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            var str = reader.ReadVarString();

            return EndPoint.Parse(str);
        }

        public int Serialize(IBinarySerializer binarySerializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            if (value is EndPoint ep)
            {
                return writer.WriteVarString(ep.ToString());
            }

            throw new ArgumentException(nameof(value));
        }
    }
}