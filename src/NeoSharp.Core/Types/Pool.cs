using System;
using System.Collections.Generic;

namespace NeoSharp.Core.Types
{
    public class Pool<TKey, TValue> where TKey : IEquatable<TKey>
    {
        class Entry : IEquatable<TKey>
        {
            public TKey Key;
            public TValue Value;

            public bool Equals(TKey other)
            {
                return Key.Equals(other);
            }

            public override string ToString()
            {
                return Key.ToString();
            }
        }

        #region Variables

        public int Count => _list.Count;
        public readonly PoolMaxBehaviour Behaviour;
        public readonly uint Max;

        private bool _isSorted;
        private readonly Func<TValue, TKey> _key;
        private readonly Comparison<TValue> _order;
        private readonly List<Entry> _list;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="behaviour">Behaviour</param>
        /// <param name="max">Max</param>
        /// <param name="key">Key</param>
        /// <param name="order">Order</param>
        public Pool(PoolMaxBehaviour behaviour, uint max, Func<TValue, TKey> key, Comparison<TValue> order)
        {
            Behaviour = behaviour;
            Max = max;
            _key = key;
            _order = order;
            _isSorted = false;
            _list = new List<Entry>();
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
                if (_list.Contains(entry)) return false;

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
    }
}