using System;
using System.Numerics;

namespace NeoSharp.VM
{
    public abstract class IntegerStackItemBase : StackItem<BigInteger>, IEquatable<IntegerStackItemBase>
    {
        /// <inheritdoc />
        public override byte[] ToByteArray() => Value.ToByteArray();

        /// <inheritdoc />
        protected IntegerStackItemBase(BigInteger value) : base(value, EStackItemType.Integer) { }

        /// <inheritdoc />
        public bool Equals(IntegerStackItemBase other) => other != null && other.Value.Equals(Value);

        /// <inheritdoc />
        public override bool Equals(StackItemBase other)
        {
            if (other is IntegerStackItemBase b)
            {
                return b.Value.Equals(Value);
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IntegerStackItemBase);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}