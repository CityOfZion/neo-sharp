using System;
using System.ComponentModel;
using System.Globalization;
using NeoSharp.Core.Caching;
using NeoSharp.Core.Models;

namespace NeoSharp.Core.Converters
{
    class TransactionTypeConverter : TypeConverter
    {
        /// <summary>
        /// Reflection cache for TransactionType
        /// </summary>
        private static readonly ReflectionCache<byte> _cache = ReflectionCache<byte>.CreateFromEnum<TransactionType>();

        // TODO: Serialize acording Type of TX

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(Transaction) || destinationType == typeof(byte[]) || destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is Transaction tx)
            {
                if (destinationType == typeof(Transaction)) return tx;

                if (destinationType == typeof(byte[]))
                {
                    throw new NotImplementedException();
                }

                if (destinationType == typeof(string))
                {
                    throw new NotImplementedException();
                }
            }

            if (value == null)
            {
                return null;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(Transaction) || sourceType == typeof(byte[]) || sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is Transaction) return value;

            if (value is byte[] bytes)
            {
                throw new NotImplementedException();
            }
            if (value is string str)
            {
                throw new NotImplementedException();
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}