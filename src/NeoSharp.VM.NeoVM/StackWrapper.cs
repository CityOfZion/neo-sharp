using System;
using System.Collections.Generic;
using System.Numerics;
using Neo.VM;
using NeoSharp.VM.NeoVM.Extensions;
using NeoSharp.VM.NeoVM.StackItems;

namespace NeoSharp.VM.NeoVM
{
    public class StackWrapper : Stack
    {
        private readonly RandomAccessStack<StackItem> _stack;

        public StackWrapper(RandomAccessStack<StackItem> stack)
        {
            _stack = stack;
        }

        public override int Count => _stack.Count;

        #region Create items

        public override ArrayStackItemBase CreateArray(IEnumerable<StackItemBase> items = null)
        {
            return new ArrayStackItem(items);
        }

        public override ArrayStackItemBase CreateStruct(IEnumerable<StackItemBase> items = null)
        {
            return new StructStackItem(items);
        }

        public override BooleanStackItemBase CreateBool(bool value)
        {
            return new BooleanStackItem(value);
        }

        public override ByteArrayStackItemBase CreateByteArray(byte[] data)
        {
            return new ByteArrayStackItem(data);
        }

        public override IntegerStackItemBase CreateInteger(BigInteger value)
        {
            return new IntegerStackItem(value);
        }

        public override IntegerStackItemBase CreateInteger(int value)
        {
            return new IntegerStackItem(value);
        }

        public override IntegerStackItemBase CreateInteger(long value)
        {
            return new IntegerStackItem(value);
        }

        public override InteropStackItemBase<T> CreateInterop<T>(T obj)
        {
            return new InteropStackItem<T>(obj);
        }

        public override MapStackItemBase CreateMap()
        {
            return new MapStackItem();
        }

        #endregion

        public override void Push(StackItemBase item)
        {
            if (!(item is INativeStackItemContainer nitem)) throw new ArgumentException(nameof(item));

            _stack.Push(nitem.NativeStackItem);
        }

        public override bool TryPeek(int index, out StackItemBase item)
        {
            if (_stack.Count <= index)
            {
                item = null;
                return false;
            }

            item = _stack.Peek(index)?.ConvertFromNative();

            return item != null;
        }

        public override bool TryPop(out StackItemBase item)
        {
            if (_stack.Count < 1)
            {
                item = null;
                return false;
            }

            item = _stack.Pop()?.ConvertFromNative();

            return item != null;
        }
    }
}