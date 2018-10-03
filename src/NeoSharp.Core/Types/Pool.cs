using System;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.Types;

namespace NeoSharp.Core.Types
{
    public class Pool<TKey, TValue> where TKey : IEquatable<TKey>
    {
        class EntryComparer : IEqualityComparer<Entry>
        {
            public bool Equals(Entry x, Entry y)
            {
                return x.Key.Equals(y.Key);
            }

            public int GetHashCode(Entry obj)
            {
                return 0;
            }
        }

        class Entry : IEquatable<TKey>
        {
            /// <summary>
            /// Key
            /// </summary>
            public TKey Key;

            /// <summary>
            /// Value
            /// </summary>
            public TValue Value;

            /// <summary>
            /// Check if is equal by the key
            /// </summary>
            /// <param name="other">Other</param>
            /// <returns>Return true if is equal</returns>
            public bool Equals(TKey other)
            {
                return Key.Equals(other);
            }

            /// <summary>
            /// Return equal value
            /// </summary>
            public override int GetHashCode()
            {
                return 0;
            }

            /// <summary>
            /// String representation
            /// </summary>
            public override string ToString()
            {
                return Key.ToString();
            }
        }

        #region Private fields

        private bool _isSorted;
        private readonly Func<TValue, TKey> _key;
        private readonly Comparison<TValue> _order;
        private readonly List<Entry> _list;
        private readonly EntryComparer _comparer;

        #endregion

        #region Public fields

        public int Count => _list.Count;
        public readonly PoolMaxBehaviour Behaviour;
        public int Max { get; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="behaviour">Behaviour</param>
        /// <param name="max">Max</param>
        /// <param name="key">Key</param>
        /// <param name="order">Order</param>
        public Pool(PoolMaxBehaviour behaviour, int max, Func<TValue, TKey> key, Comparison<TValue> order)
        {
            if (max <= 0) throw new ArgumentException(nameof(max));

            Behaviour = behaviour;
            Max = max;
            _key = key;
            _order = order;
            _isSorted = false;
            _list = new List<Entry>();
            _comparer = new EntryComparer();
        }

        /// <summary>
        /// Sort
        /// </summary>
        private void Sort()
        {
            if (_order == null || _isSorted) return;

            _list.Sort((a, b) => _order(a.Value, b.Value));
            _isSorted = true;
        }

        /// <summary>
        /// Clear
        /// </summary>
        public void Clear()
        {
            lock (_list)
            {
                _list.Clear();
                _isSorted = true;
            }
        }

        /// <summary>
        /// Push
        /// </summary>
        /// <param name="value">Value</param>
        public bool Push(TValue value)
        {
            Entry entry = new Entry()
            {
                Key = _key(value),
                Value = value
            };

            lock (_list)
            {
                if (_list.Contains(entry, _comparer)) return false;

                // Add

                _isSorted = false;

                if (_list.Count < Max)
                {
                    _list.Add(entry);
                    return true;
                }

                // Overflow

                switch (Behaviour)
                {
                    case PoolMaxBehaviour.RemoveFromEnd:
                        {
                            _list.Add(entry);

                            Sort();

                            int ic = _list.Count - 1;

                            if (_list[ic] == entry)
                            {
                                _list.RemoveAt(ic);
                                return false;
                            }

                            _list.RemoveAt(ic);
                            return true;
                        }
                    default: return false;
                }
            }
        }

        /// <summary>
        /// Peek x elements from the pool
        /// </summary>
        /// <param name="count">Count</param>
        /// <returns>Return array of elements</returns>
        public TValue[] Peek(int count = 1)
        {
            lock (_list)
            {
                Sort();

                int max = Math.Min(count, Count);
                TValue[] ret = new TValue[max];

                for (int x = 0; x < max; x++)
                    ret[x] = _list[x].Value;

                return ret;
            }
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
        /// <param name="hash"></param>
        /// <returns>Return true if is removed</returns>
        public bool Remove(UInt256 hash)
        {
            lock (_list)
            {
                return _list.RemoveAll(a => a.Key.Equals(hash)) > 0;
            }
        }

        /// <summary>
        /// Pop x elements from the pool
        /// </summary>
        /// <param name="count">Count</param>
        /// <returns>Return array of elements</returns>
        public TValue[] Pop(int count = 1)
        {
            lock (_list)
            {
                TValue[] ret = Peek(count);
                _list.RemoveRange(0, ret.Length);
                return ret;
            }
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