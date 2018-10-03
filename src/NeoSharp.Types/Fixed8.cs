using System;
using System.ComponentModel;
using System.Globalization;
using NeoSharp.BinarySerialization;
using NeoSharp.Types.Converters;

namespace NeoSharp.Types
{
    /// <summary>
    /// Accurate to 10^-8 64-bit fixed-point numbers minimize rounding errors.
    /// By controlling the accuracy of the multiplier, rounding errors can be completely eliminated.
    /// </summary>
    [TypeConverter(typeof(Fixed8TypeConverter))]
    [BinaryTypeSerializer(typeof(Fixed8TypeConverter))]
    public struct Fixed8 : IComparable<Fixed8>, IEquatable<Fixed8>, IFormattable
    {
        #region Private Fields 

        private const long D = 100_000_000;
        private const ulong QUO = (1ul << 63) / (D >> 1);
        private const ulong REM = ((1ul << 63) % (D >> 1)) << 1;

        #endregion

        #region Public Constants

        public const int Size = sizeof(long);

        public static readonly Fixed8 MaxValue = new Fixed8(long.MaxValue);

        public static readonly Fixed8 MinValue = new Fixed8(long.MinValue);

        public static readonly Fixed8 One = new Fixed8(D);

        public static readonly Fixed8 Satoshi = new Fixed8(1);

        public static readonly Fixed8 Zero = default(Fixed8);

        #endregion

        #region Public Properties

        public readonly long Value;

        #endregion

        #region Constructor 

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value</param>
        public Fixed8(long value)
        {
            Value = value;
        }

        #endregion

        #region IComparable Implementation 

        public int CompareTo(Fixed8 other)
        {
            return Value.CompareTo(other.Value);
        }

        #endregion

        #region IEquatable Implementation 

        public bool Equals(Fixed8 other)
        {
            return Value.Equals(other.Value);
        }

        #endregion

        #region IFormatable Implementation 

        public string ToString(string format, IFormatProvider formatProvider)
        {
            return ((decimal)this).ToString(format, formatProvider);
        }

        #endregion

        #region Override Methods

        public override bool Equals(object obj)
        {
            if (!(obj is Fixed8 value)) return false;

            return Equals(value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return ((decimal)this).ToString(CultureInfo.InvariantCulture);
        }

        #endregion

        #region Public Methods

        public string ToString(string format)
        {
            return ((decimal)this).ToString(format);
        }

        public static Fixed8 Parse(string s)
        {
            return FromDecimal(decimal.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture));
        }

        public static bool TryParse(string s, out Fixed8 result)
        {
            if (!decimal.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal d))
            {
                result = default(Fixed8);
                return false;
            }

            d *= D;

            if (d < long.MinValue || d > long.MaxValue)
            {
                result = default(Fixed8);
                return false;
            }

            result = new Fixed8((long)d);
            return true;
        }

        public Fixed8 Abs()
        {
            if (Value >= 0) return this;
            return new Fixed8(-Value);
        }

        public Fixed8 Ceiling()
        {
            var remainder = Value % D;

            if (remainder == 0) return this;
            if (remainder > 0) return new Fixed8(Value - remainder + D);

            return new Fixed8(Value - remainder);
        }

        public static Fixed8 FromDecimal(decimal value)
        {
            value *= D;

            if (value < long.MinValue || value > long.MaxValue)
            {
                throw new OverflowException();
            }

            return new Fixed8((long)value);
        }

        public static explicit operator decimal(Fixed8 value)
        {
            return value.Value / (decimal)D;
        }

        public static explicit operator long(Fixed8 value)
        {
            return value.Value / D;
        }

        public static bool operator ==(Fixed8 x, Fixed8 y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Fixed8 x, Fixed8 y)
        {
            return !x.Equals(y);
        }

        public static bool operator >(Fixed8 x, Fixed8 y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator <(Fixed8 x, Fixed8 y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >=(Fixed8 x, Fixed8 y)
        {
            return x.CompareTo(y) >= 0;
        }

        public static bool operator <=(Fixed8 x, Fixed8 y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static Fixed8 operator *(Fixed8 x, Fixed8 y)
        {
            var sign = Math.Sign(x.Value) * Math.Sign(y.Value);
            var ux = (ulong)Math.Abs(x.Value);
            var uy = (ulong)Math.Abs(y.Value);
            var xh = ux >> 32;
            var xl = ux & 0x00000000fffffffful;
            var yh = uy >> 32;
            var yl = uy & 0x00000000fffffffful;
            var rh = xh * yh;
            var rm = xh * yl + xl * yh;
            var rl = xl * yl;
            var rmh = rm >> 32;
            var rml = rm << 32;

            rh += rmh;
            rl += rml;

            if (rl < rml) ++rh;
            if (rh >= D) throw new OverflowException();

            var rd = rh * REM + rl;

            if (rd < rl) ++rh;

            var r = rh * QUO + rd / D;

            return new Fixed8((long)r * sign);
        }

        public static Fixed8 operator *(Fixed8 x, long y)
        {
            return new Fixed8(checked(x.Value * y));
        }

        public static Fixed8 operator /(Fixed8 x, long y)
        {
            return new Fixed8(x.Value / y);
        }

        public static Fixed8 operator +(Fixed8 x, Fixed8 y)
        {
            return new Fixed8(checked(x.Value + y.Value));
        }

        public static Fixed8 operator -(Fixed8 x, Fixed8 y)
        {
            return new Fixed8(checked(x.Value - y.Value));
        }

        public static Fixed8 operator -(Fixed8 value)
        {
            return new Fixed8(-value.Value);
        }

        #endregion
    }
}
