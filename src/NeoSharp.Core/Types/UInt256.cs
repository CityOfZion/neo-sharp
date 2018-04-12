using System;
using System.Collections;
using System.IO;
using System.Linq;
using NeoSharp.Core.Extensions;

namespace NeoSharp.Core.Types
{
    public class UInt256 : IEquatable<UInt256>, IComparable<UInt256>, ISerializable
    {
        private static readonly int s_size = 32;

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

        public int Size => s_size;

        public bool Equals(UInt256 other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return _buffer.SequenceEqual(other._buffer);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
                return false;
            if (obj is UInt256 other)
                return Equals(other);
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

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(_buffer);
        }

        public void Deserialize(BinaryReader reader)
        {
            reader.Read(_buffer, 0, _buffer.Length);
        }

        public byte[] ToArray()
        {
            return _buffer.ToArray();
        }

        public override string ToString()
        {
            return "0x" + _buffer.Reverse().ToHexString();
        }

        public static UInt256 Parse(string value)
        {
            return new UInt256(value.HexToBytes(s_size * 2).Reverse().ToArray());
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
            return ReferenceEquals(left, null) == false
                ? left.Equals(right)
                : ReferenceEquals(right, null);
        }

        public static bool operator !=(UInt256 left, UInt256 right)
        {
            return !(left == right);
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
