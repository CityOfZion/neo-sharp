using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using NeoSharp.BinarySerialization;
using NeoSharp.BinarySerialization.SerializationHooks;

namespace NeoSharp.Types.Converters
{
    class UInt160Converter : TypeConverter, IBinaryCustomSerializable
    {
        public static readonly int FixedLength = UInt160.BufferLength;

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(UInt160) || destinationType == typeof(byte[]) || destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is UInt160 uint160)
            {
                if (destinationType == typeof(UInt160)) return uint160;
                if (destinationType == typeof(byte[])) return uint160.ToArray();
                if (destinationType == typeof(string)) return uint160.ToString();
            }

            if (value == null)
            {
                return null;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(UInt160) || sourceType == typeof(byte[]) || sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is UInt160) return value;
            if (value is byte[] bytes && bytes.Length == UInt160.Zero.Size) return new UInt160(bytes);
            if (value is string str) return UInt160.Parse(str);

            return base.ConvertFrom(context, culture, value);
        }

        public object Deserialize(IBinarySerializer binaryDeserializer, BinaryReader reader, Type type, BinarySerializerSettings settings = null)
        {
            var val = new byte[FixedLength];
            reader.Read(val, 0, FixedLength);

            return new UInt160(val);
        }

        public int Serialize(IBinarySerializer binarySerializer, BinaryWriter writer, object value, BinarySerializerSettings settings = null)
        {
            if (value is UInt160 hash)
            {
                writer.Write(hash.ToArray(), 0, FixedLength);
                return FixedLength;
            }

            throw new ArgumentException(nameof(value));
        }
    }
}