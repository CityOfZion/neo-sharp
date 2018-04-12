using NeoSharp.Core.Types;
using System;

namespace NeoSharp.Core.Caching
{
    public abstract class MetaDataCache<T> where T : class, ISerializable, new()
    {
        protected T Item;
        protected TrackState State;
        private Func<T> _factory;

        protected abstract T TryGetInternal();

        protected MetaDataCache(Func<T> factory)
        {
            _factory = factory;
        }

        public T Get()
        {
            if (Item == null)
            {
                Item = TryGetInternal();
            }
            if (Item == null)
            {
                Item = _factory?.Invoke() ?? new T();
                State = TrackState.Added;
            }
            return Item;
        }

        public T GetAndChange()
        {
            var item = Get();
            if (State == TrackState.None)
                State = TrackState.Changed;
            return item;
        }
    }
}
