using System;
using System.Collections;
using System.Collections.Generic;

namespace NeoSharp.VM
{
    public abstract class MapStackItemBase : StackItemBase, IEnumerable<KeyValuePair<StackItemBase, StackItemBase>>
    {
        /// <summary>
        /// Count
        /// </summary>
        public abstract int Count { get; }

        public StackItemBase this[StackItemBase key]
        {
            get => TryGetValue(key, out var value) ? value : null;
            set => Set(key, value);
        }

        /// <summary>
        /// Is not possible to convert to byte array
        /// </summary>
        /// <returns>NULL</returns>
        public override byte[] ToByteArray() => null;

        public abstract IEnumerable<StackItemBase> Keys { get; }

        public abstract IEnumerable<StackItemBase> Values { get; }

        /// <inheritdoc />
        protected MapStackItemBase() : base(EStackItemType.Map) { }

        #region Write

        public abstract bool Remove(StackItemBase key);

        public abstract void Set(KeyValuePair<StackItemBase, StackItemBase> item);

        public abstract void Set(StackItemBase key, StackItemBase value);

        public abstract void Clear();

        #endregion

        #region Read

        /// <summary>
        /// Get raw object
        /// </summary>
        /// <returns>Raw object</returns>
        public override object ToObject()
        {
            var ret = new Dictionary<object, object>();

            foreach (var keyValue in this)
            {
                using (keyValue.Key)
                using (keyValue.Value)
                {
                    ret.Add(keyValue.Key.ToObject(), keyValue.Value.ToObject());
                }
            }

            return ret;
        }

        public abstract bool ContainsKey(StackItemBase key);

        public abstract bool TryGetValue(StackItemBase key, out StackItemBase value);

        #endregion

        #region Enumerables

        public abstract IEnumerator<KeyValuePair<StackItemBase, StackItemBase>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}