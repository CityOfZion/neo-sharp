using System;

namespace NeoSharp.Core.Types
{
    public class Stamp<T>
    {
        /// <summary>
        /// Store the creation date
        /// </summary>
        public readonly DateTime Date;

        /// <summary>
        /// Store the value
        /// </summary>
        public readonly T Value;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value</param>
        public Stamp(T value)
        {
            Value = value;
            Date = DateTime.UtcNow;
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class StampedPool<TKey, TValue> : Pool<TKey, Stamp<TValue>> where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="behaviour">Behaviour</param>
        /// <param name="max">Max</param>
        /// <param name="key">Key</param>
        /// <param name="order">Order</param>
        public StampedPool(PoolMaxBehaviour behaviour, int max, Func<Stamp<TValue>, TKey> key, Comparison<Stamp<TValue>> order)
            : base(behaviour, max, key, order) { }

        /// <summary>
        /// Push
        /// </summary>
        /// <param name="value">Value</param>
        public bool Push(TValue value)
        {
            return Push(new Stamp<TValue>(value));
        }
    }
}