using NeoSharp.Core.Extensions;
using NeoSharp.Core.Types;
using System;
using System.ComponentModel;
using System.Globalization;

namespace NeoSharp.Core.Converters
{
    class UInt160Converter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(UInt160) || sourceType == typeof(byte[]) || sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is UInt160) return value;
            if (value is byte[]) return new UInt160((byte[])value);
            if (value is string) return new UInt160(((string)value).HexToBytes());

            return base.ConvertFrom(context, culture, value);
        }
    }
}