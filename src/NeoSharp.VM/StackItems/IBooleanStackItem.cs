using System;

namespace NeoSharp.VM
{
    public abstract class IBooleanStackItem : IStackItem<bool>, IEquatable<IBooleanStackItem>
    {
        protected static readonly byte[] TRUE = { 1 };
        protected static readonly byte[] FALSE = new byte[0];

        public override bool CanConvertToByteArray => true;

        public override byte[] ToByteArray() => Value ? TRUE : FALSE;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="value">Value</param>
        public IBooleanStackItem(IExecutionEngine engine, bool value) : base(engine, value, EStackItemType.Bool) { }

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public bool Equals(IBooleanStackItem other) => other != null && other.Value == Value;

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public override bool Equals(IStackItem other)
        {
            if (other is IBooleanStackItem b)
            {
                return b.Value == Value;
            }

            return false;
        }
    }
}