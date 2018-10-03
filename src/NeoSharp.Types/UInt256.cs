using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Types.Converters;
using NeoSharp.Types.ExtensionMethods;

namespace NeoSharp.Types
{
    [TypeConverter(typeof(UInt256Converter))]
    [BinaryTypeSerializer(typeof(UInt256Converter))]
    public class UInt256 : IEquatable<UInt256>, IComparable<UInt256>
    {
        public static readonly int BufferLength = 32;

        public static readonly UInt256 Zero = new UInt256();

        private readonly byte[] _buffer;

        public UInt256()
        {
            _buffer = new byte[Size];
        }

        public UInt256(byte[] value) : this()
        {
            if (value.Length != Size)
                throw new ArgumentException();

            Array.Copy(value, _buffer, _buffer.Length);
        }

        public int Size => BufferLength;

        public bool Equals(UInt256 other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return _buffer.SequenceEqual(other._buffer);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj is UInt256 other)
            {
                return _buffer.SequenceEqual(other._buffer);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return _buffer.ToInt32(0);
        }

        public int CompareTo(UInt256 other)
        {
            return ((IStructuralComparable)_buffer).CompareTo(other._buffer, StructuralComparisons.StructuralComparer);
        }

        public byte[] ToArray()
        {
            return _buffer.ToArray();
        }

        public override string ToString()
        {
            return _buffer.Reverse().ToHexString(true);
        }

        public string ToString(bool append0x)
        {
            return _buffer.Reverse().ToHexString(append0x);
        }

        public static UInt256 Parse(string value)
        {
            return new UInt256(value.HexToBytes(BufferLength * 2).Reverse().ToArray());
        }

        public static bool TryParse(string s, out UInt256 result)
        {
            try
            {
                result = Parse(s);
                return true;
            }
            catch
            {
                result = Zero;
                return false;
            }
        }

        public static bool operator ==(UInt256 left, UInt256 right)
        {
            return left is null ? right is null : left.Equals(right);
        }

        public static bool operator !=(UInt256 left, UInt256 right)
        {
            return !(left is null ? right is null : left.Equals(right));
        }

        public static bool operator >(UInt256 left, UInt256 right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(UInt256 left, UInt256 right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <(UInt256 left, UInt256 right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(UInt256 left, UInt256 right)
        {
            return left.CompareTo(right) <= 0;
        }
    }
}
