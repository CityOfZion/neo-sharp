using System;
using Neo.VM;
using NeoSharp.VM.NeoVM.Extensions;

namespace NeoSharp.VM.NeoVM
{
    public class StackExecutionContextWrapper : StackBase<ExecutionContextBase>
    {
        private readonly RandomAccessStack<ExecutionContext> _stack;

        public StackExecutionContextWrapper(RandomAccessStack<ExecutionContext> invocationStack)
        {
            _stack = invocationStack;
        }

        public override int Count => _stack.Count;

        public override void Push(ExecutionContextBase item)
        {
            if (!(item is ExecutionContextWrapper nitem)) throw new ArgumentException(nameof(item));

            _stack.Push(nitem.NativeContext);
        }

        public override bool TryPeek(int index, out ExecutionContextBase item)
        {
            if (_stack.Count <= index)
            {
                item = null;
                return false;
            }

            item = _stack.Peek(index).ConvertFromNative();

            return true;
        }

        public override bool TryPop(out ExecutionContextBase item)
        {
            if (_stack.Count < 1)
            {
                item = null;
                return false;
            }

            item = _stack.Pop().ConvertFromNative();

            return true;
        }
    }
}