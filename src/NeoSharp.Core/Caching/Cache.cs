using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeoSharp.Core.Caching
{
    internal abstract class Cache<TKey, TValue> : ICollection<TValue>, IDisposable
    {
        protected class CacheItem
        {
            public TKey Key;
            public TValue Value;
            public DateTime Time;

            public CacheItem(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Time = DateTime.UtcNow;
            }
        }

        public readonly object SyncRoot = new object();
        protected readonly Dictionary<TKey, CacheItem> InnerDictionary = new Dictionary<TKey, CacheItem>();
        private readonly int _maxCapacity;

        public TValue this[TKey key]
        {
            get
            {
                lock (SyncRoot)
                {
                    if (!InnerDictionary.TryGetValue(key, out var item)) throw new KeyNotFoundException();
                    OnAccess(item);
                    return item.Value;
                }
            }
        }

        public int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return InnerDictionary.Count;
                }
            }
        }

        public bool IsReadOnly => false;

        public Cache(int maxCapacity)
        {
            _maxCapacity = maxCapacity;
        }

        public void Add(TValue item)
        {
            var key = GetKeyForItem(item);
            lock (SyncRoot)
            {
                AddInternal(key, item);
            }
        }

        private void AddInternal(TKey key, TValue item)
        {
            if (InnerDictionary.TryGetValue(key, out var cacheItem))
            {
                OnAccess(cacheItem);
            }
            else
            {
                if (InnerDictionary.Count >= _maxCapacity)
                {
                    //TODO: Perform a performance test on the PLINQ query to determine which algorithm to use here is better (parallel or serial)
                    foreach (var itemDel in InnerDictionary.Values.AsParallel().OrderBy(p => p.Time).Take(InnerDictionary.Count - _maxCapacity + 1))
                    {
                        RemoveInternal(itemDel);
                    }
                }
                InnerDictionary.Add(key, new CacheItem(key, item));
            }
        }

        public void AddRange(IEnumerable<TValue> items)
        {
            lock (SyncRoot)
            {
                foreach (var item in items)
                {
                    var key = GetKeyForItem(item);
                    AddInternal(key, item);
                }
            }
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                foreach (var itemDel in InnerDictionary.Values.ToArray())
                {
                    RemoveInternal(itemDel);
                }
            }
        }

        public bool Contains(TKey key)
        {
            lock (SyncRoot)
            {
                if (!InnerDictionary.TryGetValue(key, out var cacheItem)) return false;
                OnAccess(cacheItem);
                return true;
            }
        }

        public bool Contains(TValue item)
        {
            return Contains(GetKeyForItem(item));
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException();
            if (arrayIndex < 0) throw new ArgumentOutOfRangeException();
            lock (SyncRoot)
            {
                if (arrayIndex + InnerDictionary.Count > array.Length) throw new ArgumentException();
            }
            foreach (var item in this)
            {
                array[arrayIndex++] = item;
            }
        }

        public void Dispose()
        {
            Clear();
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            lock (SyncRoot)
            {
                foreach (var item in InnerDictionary.Values.Select(p => p.Value))
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected abstract TKey GetKeyForItem(TValue item);

        public bool Remove(TKey key)
        {
            lock (SyncRoot)
            {
                if (!InnerDictionary.TryGetValue(key, out var cacheItem)) return false;
                RemoveInternal(cacheItem);
                return true;
            }
        }

        protected abstract void OnAccess(CacheItem item);

        public bool Remove(TValue item)
        {
            return Remove(GetKeyForItem(item));
        }

        private void RemoveInternal(CacheItem item)
        {
            InnerDictionary.Remove(item.Key);
            if (item.Value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public bool TryGet(TKey key, out TValue item)
        {
            lock (SyncRoot)
            {
                if (InnerDictionary.TryGetValue(key, out var cacheItem))
                {
                    OnAccess(cacheItem);
                    item = cacheItem.Value;
                    return true;
                }
            }
            item = default(TValue);
            return false;
        }
    }
}
