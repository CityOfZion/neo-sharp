using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NeoSharp.VM
{
    public abstract class Stack : StackBase<StackItemBase>
    {
        #region Creates

        /// <summary>
        /// Create Map StackItem
        /// </summary>
        protected internal abstract MapStackItemBase CreateMap();

        /// <summary>
        /// Create Array StackItem
        /// </summary>
        /// <param name="items">Items</param>
        protected internal abstract ArrayStackItemBase CreateArray(IEnumerable<StackItemBase> items = null);

        /// <summary>
        /// Create Struct StackItem
        /// </summary>
        /// <param name="items">Items</param>
        protected internal abstract ArrayStackItemBase CreateStruct(IEnumerable<StackItemBase> items = null);

        /// <summary>
        /// Create ByteArrayStackItem
        /// </summary>
        /// <param name="data">Buffer</param>
        protected internal abstract ByteArrayStackItemBase CreateByteArray(byte[] data);

        /// <summary>
        /// Create InteropStackItem
        /// </summary>
        /// <param name="obj">Object</param>
        protected internal abstract InteropStackItemBase<T> CreateInterop<T>(T obj) where T : class;

        /// <summary>
        /// Create BooleanStackItem
        /// </summary>
        /// <param name="value">Value</param>
        protected internal abstract BooleanStackItemBase CreateBool(bool value);

        /// <summary>
        /// Create IntegerStackItem
        /// </summary>
        /// <param name="value">Value</param>
        protected internal abstract IntegerStackItemBase CreateInteger(int value);

        /// <summary>
        /// Create IntegerStackItem
        /// </summary>
        /// <param name="value">Value</param>
        protected internal abstract IntegerStackItemBase CreateInteger(long value);

        /// <summary>
        /// Create IntegerStackItem
        /// </summary>
        /// <param name="value">Value</param>
        protected internal abstract IntegerStackItemBase CreateInteger(BigInteger value);

        #endregion

        public void Push(bool value) => Push(CreateBool(value));

        public void Push(int value) => Push(CreateInteger(value));

        public void Push(uint value) => Push(CreateInteger(value));

        public void Push(long value) => Push(CreateInteger(value));

        public void Push(ulong value) => Push(CreateInteger(value));

        public void Push(byte[] value) => Push(CreateByteArray(value));

        public void PushObject<T>(T item) where T : class => Push(CreateInterop(item));

        public void PushArray<T>(T[] items) where T : class
        {
            var stackItems = items
                .Select(CreateInterop)
                .ToArray();

            Push(CreateArray(stackItems));
        }

        public byte[] PeekByteArray(int index = 0)
        {
            var stackItem = Peek(index) as ByteArrayStackItemBase;

            return stackItem?.Value;
        }

        public T PeekObject<T>(int index = 0) where T : class
        {
            var stackItem = Peek(index) as InteropStackItemBase<T>;

            return stackItem?.Value;
        }

        public BigInteger? PopBigInteger()
        {
            var stackItem = Pop() as IntegerStackItemBase;

            using (stackItem)
            {
                return stackItem?.Value;
            }
        }

        public byte[] PopByteArray()
        {
            var stackItem = Pop() as ByteArrayStackItemBase;

            using (stackItem)
            {
                return stackItem?.Value;
            }
        }

        public T PopObject<T>() where T : class
        {
            var stackItem = Pop();

            if (stackItem is InteropStackItemBase<T> interop)
            {
                using (stackItem)
                {
                    return interop.Value;
                }
            }
            
            // Extract base type
            if (stackItem is T obj)
            {
                return obj;
            }

            throw new ArgumentException(nameof(T));
        }

        public T[] PopArray<T>() where T : class
        {
            var stackItems = Pop() as ArrayStackItemBase;

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