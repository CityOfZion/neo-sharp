using System;
using System.Collections.Generic;
using Neo.VM;
using NeoSharp.VM.NeoVM.Extensions;

namespace NeoSharp.VM.NeoVM.StackItems
{
    public class ArrayStackItem : ArrayStackItemBase, INativeStackItemContainer
    {
        public StackItem NativeStackItem => _item;

        public override int Count => _item.Count;

        public override bool IsDisposed => false;

        private readonly Neo.VM.Types.Array _item;

        public override StackItemBase this[int index]
        {
            get => _item[index].ConvertFromNative();
            set
            {
                if (!(value is INativeStackItemContainer nitem)) throw new ArgumentException(nameof(value));

                _item[index] = nitem.NativeStackItem;
            }
        }

        public ArrayStackItem(Neo.VM.Types.Array item) : base(false)
        {
            _item = item;
        }

        public ArrayStackItem(IEnumerable<StackItemBase> items = null) : base(true)
        {
            _item = new Neo.VM.Types.Struct();

            if (items != null)
            {
                Add(items);
            }
        }

        public override void Add(StackItemBase item)
        {
            if (!(item is INativeStackItemContainer nitem)) throw new ArgumentException(nameof(item));

            _item.Add(nitem.NativeStackItem);
        }

        public override void Add(params StackItemBase[] items)
        {
            foreach (var item in items) Add(item);
        }

        public override void Add(IEnumerable<StackItemBase> items)
        {
            foreach (var item in items) Add(item);
        }

        public override void Clear()
        {
            _item.Clear();
        }

        public override void Insert(int index, StackItemBase item)
        {
            if (!(item is INativeStackItemContainer nitem)) throw new ArgumentException(nameof(item));

            _item.Insert(index, nitem.NativeStackItem);
        }

        public override void Set(int index, StackItemBase item)
        {
            if (!(item is INativeStackItemContainer nitem)) throw new ArgumentException(nameof(item));

            _item[index] = nitem.NativeStackItem;
        }

        public override void RemoveAt(int index)
        {
            _item.RemoveAt(index);
        }

        public override int IndexOf(StackItemBase item)
        {
            if (!(item is INativeStackItemContainer nitem)) return -1;

            for (var x = 0; x < _item.Count; x++)
            {
                if (_item[x].Equals(nitem.NativeStackItem)) return x;
            }

            return -1;
        }
    }
}