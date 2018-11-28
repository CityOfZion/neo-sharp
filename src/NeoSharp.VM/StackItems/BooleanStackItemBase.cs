using System;

namespace NeoSharp.VM
{
    public abstract class BooleanStackItemBase : StackItem<bool>, IEquatable<BooleanStackItemBase>
    {
        protected static readonly byte[] True = { 1 };
        protected static readonly byte[] False = Array.Empty<byte>();

        public override byte[] ToByteArray() => Value ? True : False;

        /// <inheritdoc />
        protected BooleanStackItemBase(bool value) : base(value, EStackItemType.Bool) { }

        /// <inheritdoc />
        public bool Equals(BooleanStackItemBase other) => other != null && other.Value == Value;

        /// <inheritdoc />
        public override bool Equals(StackItemBase other)
        {
            if (other is BooleanStackItemBase b)
            {
                return b.Value == Value;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as BooleanStackItemBase);
        }
    }
}