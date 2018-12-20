using System;

namespace NeoSharp.VM
{
    public abstract class StackItem<T> : StackItemBase
    {
        /// <summary>
        /// Value
        /// </summary>
        public T Value { get; }

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="type">Type</param>
        protected StackItem(T data, EStackItemType type) : base(type)
        {
            Value = data;
        }

        /// <inheritdoc />
        public override object ToObject()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Value);
        }
    }
}