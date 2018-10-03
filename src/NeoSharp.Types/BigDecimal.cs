using System;
using System.Numerics;

namespace NeoSharp.Types
{
    public struct BigDecimal
    {
        private readonly BigInteger _value;
        private readonly byte _decimals;

        public BigInteger Value => _value;
        public byte Decimals => _decimals;
        public int Sign => _value.Sign;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="decimals">Number of decimals</param>
        public BigDecimal(BigInteger value, byte decimals)
        {
            _value = value;
            _decimals = decimals;
        }

        /// <summary>
        /// Change the numbero of decimals
        /// </summary>
        /// <param name="decimals">New decimals</param>
        /// <returns>Returns a new BigDecimal with the decimals desired</returns>
        public BigDecimal ChangeDecimals(byte decimals)
        {
            if (_decimals == decimals) return this;
            BigInteger value;
            if (_decimals < decimals)
            {
                value = _value * BigInteger.Pow(10, decimals - _decimals);
            }
            else
            {
                var divisor = BigInteger.Pow(10, _decimals - decimals);
                value = BigInteger.DivRem(_value, divisor, out var remainder);
                if (remainder > BigInteger.Zero)
                    throw new ArgumentOutOfRangeException();
            }
            return new BigDecimal(value, decimals);
        }

        /// <summary>
        /// Parse string to BigDecimal
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="decimals">Number of decimals</param>
        /// <returns>Returns a new BigDecimal</returns>
        public static BigDecimal Parse(string s, byte decimals)
        {
            if (!TryParse(s, decimals, out var result))
                throw new FormatException();
            return result;
        }

        /// <summary>
        /// Convert to Fixed8 type
        /// </summary>
        /// <returns>Returns a new Fixed8</returns>
        public Fixed8 ToFixed8()
        {
            try
            {
                return new Fixed8((long)ChangeDecimals(8)._value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Convert to String 
        /// </summary>
        /// <returns>Returns a string with the number representation</returns>
        public override string ToString()
        {
            var divisor = BigInteger.Pow(10, _decimals);
            var result = BigInteger.DivRem(_value, divisor, out var remainder);
            if (remainder == 0) return result.ToString();
            return $"{result}.{remainder.ToString("d" + _decimals)}".TrimEnd('0');
        }

        /// <summary>
        /// Try to parse string to BigDecimal
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="decimals">Number of decimals</param>
        /// <param name="result">Receives the BigDecimal created</param>
        /// <returns>Returns True if operation performed right</returns>
        public static bool TryParse(string s, byte decimals, out BigDecimal result)
        {
            var e = 0;
            var index = s.IndexOfAny(new[] { 'e', 'E' });
            if (index >= 0)
            {
                if (!sbyte.TryParse(s.Substring(index + 1), out var eTemp))
                {
                    result = default(BigDecimal);
                    return false;
                }
                e = eTemp;
                s = s.Substring(0, index);
            }
            index = s.IndexOf('.');
            if (index >= 0)
            {
                s = s.TrimEnd('0');
                e -= s.Length - index - 1;
                s = s.Remove(index, 1);
            }
            var ds = e + decimals;
            if (ds < 0)
            {
                result = default(BigDecimal);
                return false;
            }
            if (ds > 0)
                s += new string('0', ds);
            if (!BigInteger.TryParse(s, out var value))
            {
                result = default(BigDecimal);
                return false;
            }
            result = new BigDecimal(value, decimals);
            return true;
        }
    }
}