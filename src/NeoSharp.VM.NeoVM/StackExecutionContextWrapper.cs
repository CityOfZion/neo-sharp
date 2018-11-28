using System;
using Neo.VM;
using NeoSharp.VM.NeoVM.Extensions;

namespace NeoSharp.VM.NeoVM
{
    public class StackExecutionContextWrapper : StackBase<ExecutionContext>
    {
        private readonly RandomAccessStack<Neo.VM.ExecutionContext> _stack;

        public StackExecutionContextWrapper(RandomAccessStack<Neo.VM.ExecutionContext> invocationStack)
        {
            _stack = invocationStack;
        }

        public override int Count => _stack.Count;

        public override int Drop(int count = 0)
        {
            count = Math.Min(count, _stack.Count);

            for (int x = 0; x < count; x++)
            {
                _stack.Pop();
            }

            return count;
        }

        public override ExecutionContext Pop()
        {
            return _stack.Pop().ConvertFromNative();
        }

        public override void Push(ExecutionContext item)
        {
            if (!(item is ExecutionContextWrapper nitem)) throw new ArgumentException(nameof(item));

            _stack.Push(nitem.NativeContext);
        }

        public override bool TryPeek(int index, out ExecutionContext obj)
        {
            if (_stack.Count <= index)
            {
                obj = null;
                return false;
            }

            obj = _stack.Peek(index).ConvertFromNative();

            return true;
        }
    }
}