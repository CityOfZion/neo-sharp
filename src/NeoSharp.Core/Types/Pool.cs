using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoSharp.Core.Types
{
    public class Pool<TKey, TValue> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        #region Private fields

        private readonly Func<TValue, TKey> _keySelector;
        private readonly IDictionary<TKey, TValue> _dictionary;

        #endregion

        #region Public fields

        public int Count => _dictionary.Count;
        public readonly PoolMaxBehaviour Behaviour;
        public readonly int Capacity;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="behaviour">Behaviour</param>
        /// <param name="capacity">Capacity</param>
        /// <param name="keySelector">Key Selector</param>
        public Pool(PoolMaxBehaviour behaviour, int capacity, Func<TValue, TKey> keySelector)
        {
            if (capacity <= 0) throw new ArgumentException(nameof(capacity));

            Behaviour = behaviour;
            Capacity = capacity;
            _keySelector = keySelector;
            _dictionary = new Dictionary<TKey, TValue>(capacity);
        }

        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <summary>
        /// Push
        /// </summary>
        /// <param name="value">Value</param>
        public bool Push(TValue value)
        {
            var entry = new KeyValuePair<TKey, TValue>(_keySelector(value), value);

            if (_dictionary.Contains(entry)) return false;

            // Add

            if (_dictionary.Count < Capacity)
            {
                _dictionary.Add(entry);
                return true;
            }

            // Overflow

            switch (Behaviour)
            {
                case PoolMaxBehaviour.RemoveFromEnd:
                    {
                        _dictionary.Add(entry);

                        var ic = _dictionary.Keys.OrderBy(_ => _).Last();

                        if (ic.Equals(entry.Key))
                        {
                            _dictionary.Remove(ic);
                            return false;
                        }

                        _dictionary.Remove(ic);
                        return true;
                    }
                default: return false;
            }
        }

        /// <summary>
        /// Peek x elements from the pool
        /// </summary>
        /// <param name="count">Count</param>
        /// <returns>Return array of elements</returns>
        public TValue[] Peek(int count = 1)
        {
            var max = Math.Min(count, Count);
            var keys = _dictionary.Keys.OrderBy(_ => _).Take(max).ToArray();
            var ret = _dictionary.Where(x => keys.Contains(x.Key)).OrderBy(x => x.Key).Select(x => x.Value).ToArray();

            return ret;
        }

        /// <summary>
        /// Peek one elements from the pool
        /// </summary>
        /// <returns>Return the element</returns>
        public TValue PeekFirstOrDefault()
        {
            return Peek(1).FirstOrDefault();
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Return true if is removed</returns>
        public bool Remove(TKey key)
        {
            return _dictionary.Remove(key);
        }

        /// <summary>
        /// Pop x elements from the pool
        /// </summary>
        /// <param name="count">Count</param>
        /// <returns>Return array of elements</returns>
        public TValue[] Pop(int count = 1)
        {
            var ret = Peek(count);

            foreach (var value in ret)
            {
                _dictionary.Remove(_keySelector(value));
            }

            return ret;
        }

        /// <summary>
        /// Pop one element from the pool
        /// </summary>
        /// <returns>Return the element</returns>
        public TValue PopFirstOrDefault()
        {
            return Pop(1).FirstOrDefault();
        }
    }
}