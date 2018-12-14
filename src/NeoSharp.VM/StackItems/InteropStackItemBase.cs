using System;

namespace NeoSharp.VM
{
    public abstract class InteropStackItemBase<T> : StackItem<T>, IEquatable<InteropStackItemBase<T>> where T : class
    {
        /// <inheritdoc />
        protected InteropStackItemBase(T value) : base(value, EStackItemType.Interop) { }

        /// <inheritdoc />
        public bool Equals(InteropStackItemBase<T> other) => other != null && other.Value.Equals(Value);

        /// <inheritdoc />
        public override bool Equals(StackItemBase other)
        {
            if (other is InteropStackItemBase<T> b)
            {
                return b.Value.Equals(Value);
            }

            return false;
        }

        /// <inheritdoc />
        public override byte[] ToByteArray() => null;

        /// <summary>
        /// Converter
        /// </summary>
        /// <param name="item">Item</param>
        public static implicit operator T(InteropStackItemBase<T> item)
        {
            return item.Value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as InteropStackItemBase<T>);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}