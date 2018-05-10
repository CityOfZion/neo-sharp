using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using System;
using System.ComponentModel;
using System.Globalization;

namespace NeoSharp.Core.Converters
{
    class UInt256Converter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(UInt256) || sourceType == typeof(byte[]) || sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is UInt256) return value;
            if (value is byte[]) return new UInt256((byte[])value);
            if (value is string) return new UInt256(((string)value).HexToBytes());

            return base.ConvertFrom(context, culture, value);
        }
    }
}