using System;
using System.Collections.Generic;
using Neo.VM;
using NeoSharp.VM.NeoVM.Extensions;

namespace NeoSharp.VM.NeoVM.StackItems
{
    public class MapStackItem : MapStackItemBase, INativeStackItemContainer
    {
        public StackItem NativeStackItem => _item;

        public override bool IsDisposed => false;

        public override int Count => _item.Count;

        private readonly Neo.VM.Types.Map _item;

        public MapStackItem(Neo.VM.Types.Map item)
        {
            _item = item;
        }

        public MapStackItem()
        {
            _item = new Neo.VM.Types.Map();
        }

        public override bool Remove(StackItemBase key)
        {
            if (!(key is INativeStackItemContainer nitem)) throw new ArgumentException(nameof(key));

            return _item.Remove(nitem.NativeStackItem);
        }

        public override void Set(KeyValuePair<StackItemBase, StackItemBase> item)
        {
            if (!(item.Key is INativeStackItemContainer nkey)) throw new ArgumentException(nameof(item));
            if (!(item.Value is INativeStackItemContainer nvalue)) throw new ArgumentException(nameof(item));

            _item[nkey.NativeStackItem] = nvalue.NativeStackItem;
        }

        public override void Set(StackItemBase key, StackItemBase value)
        {
            if (!(key is INativeStackItemContainer nkey)) throw new ArgumentException(nameof(key));
            if (!(value is INativeStackItemContainer nvalue)) throw new ArgumentException(nameof(value));

            _item[nkey.NativeStackItem] = nvalue.NativeStackItem;
        }

        public override void Clear() => _item.Clear();

        public override bool ContainsKey(StackItemBase key)
        {
            if (!(key is INativeStackItemContainer nkey)) throw new ArgumentException(nameof(key));

            return _item.ContainsKey(nkey.NativeStackItem);
        }

        public override bool TryGetValue(StackItemBase key, out StackItemBase value)
        {
            if (!(key is INativeStackItemContainer nkey)) throw new ArgumentException(nameof(key));

            if (_item.TryGetValue(nkey.NativeStackItem, out var nvalue))
            {
                value = nvalue.ConvertFromNative();
                return true;
            }

            value = null;
            return false;
        }

        public override IEnumerable<StackItemBase> Keys
        {
            get
            {
                foreach (var key in _item.Keys)
                {
                    yield return key.ConvertFromNative();
                }
            }
        }

        public override IEnumerable<StackItemBase> Values
        {
            get
            {
                foreach (var val in _item.Values)
                {
                    yield return val.ConvertFromNative();
                }
            }
        }

        public override IEnumerator<KeyValuePair<StackItemBase, StackItemBase>> GetEnumerator()
        {
            foreach (var keyValuePair in _item)
            {
                yield return new KeyValuePair<StackItemBase, StackItemBase>(keyValuePair.Key.ConvertFromNative(), keyValuePair.Value.ConvertFromNative());
            }
        }
    }
}