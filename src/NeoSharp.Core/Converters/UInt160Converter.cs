using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using System;
using System.ComponentModel;
using System.Globalization;

namespace NeoSharp.Core.Converters
{
    class UInt160Converter : TypeConverter
    {
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
            if (value is string str) return new UInt160(str.HexToBytes());

            return base.ConvertFrom(context, culture, value);
        }
    }
}