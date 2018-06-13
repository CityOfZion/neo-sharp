using System;
using System.ComponentModel;
using System.Globalization;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.Converters
{
    class Fixed8TypeConverter : TypeConverter, IFixedBufferConverter
    {
        /// <summary>
        /// Buffer length
        /// </summary>
        public int FixedLength => 8;

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Fixed8) || destinationType == typeof(long) || destinationType == typeof(byte[]) || destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is Fixed8 val)
            {
                if (destinationType == typeof(Fixed8)) return value;
                if (destinationType == typeof(long)) return val.Value;
                if (destinationType == typeof(byte[])) return BitConverter.GetBytes(val.Value);
                if (destinationType == typeof(string)) return val.ToString();
            }

            if (value == null)
            {
                return null;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(Fixed8) || sourceType == typeof(long) || sourceType == typeof(byte[]) || sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is Fixed8)
            {
                return value;
            }
            if (value is byte[] bytes && bytes.Length == FixedLength)
            {
                return new Fixed8(BitConverter.ToInt64(bytes, 0));
            }
            if (value is long l)
            {
                return new Fixed8(l);
            }
            if (value is string str && Fixed8.TryParse(str, out var res))
            {
                return res;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}