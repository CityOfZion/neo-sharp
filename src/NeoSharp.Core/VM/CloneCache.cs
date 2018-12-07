using System;
using System.Collections.Generic;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.VM
{
    internal class CloneCache<TKey, TValue> : DataCache<TKey, TValue>
        where TKey : IEquatable<TKey>
        where TValue : class, ICloneable<TValue>, new()
    {
        private DataCache<TKey, TValue> innerCache;

        public CloneCache(DataCache<TKey, TValue> innerCache)
        {
            this.innerCache = innerCache;
        }

        protected override void AddInternal(TKey key, TValue value)
        {
            innerCache.Add(key, value);
        }

        public override void DeleteInternal(TKey key)
        {
            innerCache.Delete(key);
        }

        protected override IEnumerable<KeyValuePair<TKey, TValue>> FindInternal(byte[] key_prefix)
        {
            foreach (KeyValuePair<TKey, TValue> pair in innerCache.Find(key_prefix))
                yield return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value.Clone());
        }

        protected override TValue TryGetInternal(TKey key)
        {
            return innerCache.TryGet(key)?.Clone();
        }

        protected override void UpdateInternal(TKey key, TValue value)
        {
            innerCache.GetAndChange(key).FromReplica(value);
        }
    }
}