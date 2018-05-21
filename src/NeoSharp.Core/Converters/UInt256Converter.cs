using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using System;
using System.ComponentModel;
using System.Globalization;

namespace NeoSharp.Core.Converters
{
    class UInt256Converter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(UInt256) || destinationType == typeof(byte[]) || destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is UInt256 uint256)
            {
                if (destinationType == typeof(UInt256)) return uint256;
                if (destinationType == typeof(byte[])) return uint256.ToArray();
                if (destinationType == typeof(string)) return uint256.ToString();
            }

            if (value == null)
            {
                return null;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(UInt256) || sourceType == typeof(byte[]) || sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is UInt256) return value;
            if (value is byte[] bytes && bytes.Length == UInt256.Zero.Size) return new UInt256(bytes);
            if (value is string str) return new UInt256(str.HexToBytes());

            return base.ConvertFrom(context, culture, value);
        }
    }
}