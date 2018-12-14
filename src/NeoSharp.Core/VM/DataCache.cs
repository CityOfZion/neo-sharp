using System;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.BinarySerialization;
using NeoSharp.Core.Types;

namespace NeoSharp.Core.VM
{
    public abstract class DataCache<TKey, TValue>
        where TKey : IEquatable<TKey>
        where TValue : class, ICloneable<TValue>, new()
    {
        protected internal class Trackable
        {
            public TKey Key;
            public TValue Item;
            public TrackState State;
        }

        private readonly Dictionary<TKey, Trackable> _dictionary = new Dictionary<TKey, Trackable>();

        public TValue this[TKey key]
        {
            get
            {
                if (_dictionary.TryGetValue(key, out var trackable))
                {
                    if (trackable.State == TrackState.Deleted)
                        throw new KeyNotFoundException();
                }
                else
                {
                    var item = TryGetInternal(key);
                    if (item == null) throw new KeyNotFoundException();

                    trackable = new Trackable
                    {
                        Key = key,
                        Item = item,
                        State = TrackState.None
                    };
                    _dictionary.Add(key, trackable);
                }
                return trackable.Item;
            }
        }

        public void Add(TKey key, TValue value)
        {
            if (_dictionary.TryGetValue(key, out var trackable) && trackable.State != TrackState.Deleted)
                throw new ArgumentException();
            _dictionary[key] = new Trackable
            {
                Key = key,
                Item = value,
                State = trackable == null ? TrackState.Added : TrackState.Changed
            };
        }

        protected abstract void AddInternal(TKey key, TValue value);

        public void Commit()
        {
            foreach (var trackable in GetChangeSet())
                switch (trackable.State)
                {
                    case TrackState.Added:
                        AddInternal(trackable.Key, trackable.Item);
                        break;
                    case TrackState.Changed:
                        UpdateInternal(trackable.Key, trackable.Item);
                        break;
                    case TrackState.Deleted:
                        DeleteInternal(trackable.Key);
                        break;
                }
        }

        public DataCache<TKey, TValue> CreateSnapshot()
        {
            return new CloneCache<TKey, TValue>(this);
        }

        public void Delete(TKey key)
        {
            if (_dictionary.TryGetValue(key, out var trackable))
            {
                if (trackable.State == TrackState.Added)
                    _dictionary.Remove(key);
                else
                    trackable.State = TrackState.Deleted;
            }
            else
            {
                var item = TryGetInternal(key);
                if (item == null) return;
                _dictionary.Add(key, new Trackable
                {
                    Key = key,
                    Item = item,
                    State = TrackState.Deleted
                });
            }
        }

        public abstract void DeleteInternal(TKey key);

        public void DeleteWhere(Func<TKey, TValue, bool> predicate)
        {
            foreach (var trackable in _dictionary.Where(p => p.Value.State != TrackState.Deleted && predicate(p.Key, p.Value.Item)).Select(p => p.Value))
                trackable.State = TrackState.Deleted;
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Find(byte[] keyPrefix = null)
        {
            foreach (var pair in FindInternal(keyPrefix ?? new byte[0]))
                if (!_dictionary.ContainsKey(pair.Key))
                    yield return pair;
            foreach (var pair in _dictionary)
                if (pair.Value.State != TrackState.Deleted && (keyPrefix == null || ToArray(pair.Key).Take(keyPrefix.Length).SequenceEqual(keyPrefix)))
                    yield return new KeyValuePair<TKey, TValue>(pair.Key, pair.Value.Item);
        }

        protected abstract IEnumerable<KeyValuePair<TKey, TValue>> FindInternal(byte[] keyPrefix);

        protected internal IEnumerable<Trackable> GetChangeSet()
        {
            return _dictionary.Values.Where(p => p.State != TrackState.None);
        }

        public TValue GetAndChange(TKey key, Func<TValue> factory = null)
        {
            if (_dictionary.TryGetValue(key, out var trackable))
            {
                if (trackable.State == TrackState.Deleted)
                {
                    if (factory == null) throw new KeyNotFoundException();
                    trackable.Item = factory();
                    trackable.State = TrackState.Changed;
                }
                else if (trackable.State == TrackState.None)
                {
                    trackable.State = TrackState.Changed;
                }
            }
            else
            {
                trackable = new Trackable
                {
                    Key = key,
                    Item = TryGetInternal(key)
                };
                if (trackable.Item == null)
                {
                    if (factory == null) throw new KeyNotFoundException();
                    trackable.Item = factory();
                    trackable.State = TrackState.Added;
                }
                else
                {
                    trackable.State = TrackState.Changed;
                }
                _dictionary.Add(key, trackable);
            }
            return trackable.Item;
        }

        public TValue GetOrAdd(TKey key, Func<TValue> factory)
        {
            if (_dictionary.TryGetValue(key, out var trackable))
            {
                if (trackable.State == TrackState.Deleted)
                {
                    trackable.Item = factory();
                    trackable.State = TrackState.Changed;
                }
            }
            else
            {
                trackable = new Trackable
                {
                    Key = key,
                    Item = TryGetInternal(key)
                };
                if (trackable.Item == null)
                {
                    trackable.Item = factory();
                    trackable.State = TrackState.Added;
                }
                else
                {
                    trackable.State = TrackState.None;
                }
                _dictionary.Add(key, trackable);
            }
            return trackable.Item;
        }

        public TValue TryGet(TKey key)
        {
            if (_dictionary.TryGetValue(key, out var trackable))
            {
                if (trackable.State == TrackState.Deleted) return null;
                return trackable.Item;
            }
            var value = TryGetInternal(key);
            if (value == null) return null;
            _dictionary.Add(key, new Trackable
            {
                Key = key,
                Item = value,
                State = TrackState.None
            });
            return value;
        }

        protected abstract TValue TryGetInternal(TKey key);

        protected abstract void UpdateInternal(TKey key, TValue value);

        private static byte[] ToArray(TKey value)
        {
            return BinarySerializer.Default.Serialize(value);
        }
    }
}