using System.Linq;
using System.Numerics;
using NeoSharp.VM;
using Stack = NeoSharp.VM.Stack;

namespace NeoSharp.Core.VM
{
    public class StackAccessor : IStackAccessor
    {
        private readonly ExecutionEngineBase _engine;
        private readonly Stack _stack;

        public StackAccessor(ExecutionEngineBase engine, Stack stack)
        {
            _engine = engine;
            _stack = stack;
        }

        public void Push(bool value) => _stack.Push(_engine.CreateBool(value));

        public void Push(int value) => _stack.Push(_engine.CreateInteger(value));

        public void Push(uint value) => _stack.Push(_engine.CreateInteger(value));

        public void Push(long value) => _stack.Push(_engine.CreateInteger(value));

        public void Push(ulong value) => _stack.Push(_engine.CreateInteger(value));

        public void Push(byte[] value) => _stack.Push(_engine.CreateByteArray(value));

        public void Push<T>(T item) where T : class => _stack.Push(_engine.CreateInterop(item));

        public void Push<T>(T[] items) where T : class
        {
            var stackItems = items
                .Select(_engine.CreateInterop)
                .ToArray();

            _stack.Push(_engine.CreateArray(stackItems));
        }

        public byte[] PeekByteArray(int index = 0)
        {
            var stackItem = _stack.Peek(index) as ByteArrayStackItemBase;

            return stackItem?.Value;
        }

        public T Peek<T>(int index = 0) where T : class
        {
            var stackItem = _stack.Peek(index) as InteropStackItemBase<T>;

            return stackItem?.Value;
        }

        public BigInteger? PopBigInteger()
        {
            var stackItem = _stack.Pop() as IntegerStackItemBase;

            using (stackItem)
            {
                return stackItem?.Value;
            }
        }

        public byte[] PopByteArray()
        {
            var stackItem = _stack.Pop() as ByteArrayStackItemBase;

            using (stackItem)
            {
                return stackItem?.Value;
            }
        }

        public T Pop<T>() where T : class
        {
            var stackItem = _stack.Pop() as InteropStackItemBase<T>;

            using (stackItem)
            {
                return stackItem?.Value;
            }
        }

        public T[] PopArray<T>() where T : class
        {
            var stackItems = _stack.Pop() as ArrayStackItemBase;

            using (stackItems)
            {
                return stackItems?
                    .Select(si =>
                    {
                        var stackItem = si as InteropStackItemBase<T>;

                        using (stackItem)
                        {
                            return stackItem?.Value;
                        }
                    })
                    .Where(v => v != null)
                    .ToArray();
            }
        }
    }
}