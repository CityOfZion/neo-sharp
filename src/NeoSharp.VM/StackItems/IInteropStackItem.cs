using System;

namespace NeoSharp.VM
{
    public abstract class IInteropStackItem : IStackItem<object>, IEquatable<IInteropStackItem>
    {
        public override bool CanConvertToByteArray => false;

        public override byte[] ToByteArray() => throw new NotImplementedException();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="value">Value</param>
        public IInteropStackItem(IExecutionEngine engine, object value) : base(engine, value, EStackItemType.Interop) { }

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public bool Equals(IInteropStackItem other) => other != null && other.Value.Equals(Value);

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public override bool Equals(IStackItem other)
        {
            if (other is IInteropStackItem b)
            {
                return b.Value.Equals(Value);
            }

            return false;
        }
    }
}