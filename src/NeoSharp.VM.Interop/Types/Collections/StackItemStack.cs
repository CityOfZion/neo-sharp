using System;
using System.Runtime.InteropServices;
using NeoSharp.VM.Interop.Extensions;
using NeoSharp.VM.Interop.Native;
using Newtonsoft.Json;

namespace NeoSharp.VM.Interop.Types.Collections
{
    public class StackItemStack : IStackItemsStack
    {
        /// <summary>
        /// Native handle
        /// </summary>
        private IntPtr _handle;

        /// <summary>
        /// Engine
        /// </summary>
        [JsonIgnore]
        private new readonly ExecutionEngine Engine;

        /// <summary>
        /// Return the number of items in the stack
        /// </summary>
        public override int Count => NeoVM.StackItems_Count(_handle);
        /// <summary>
        /// Drop object from the stack
        /// </summary>
        /// <param name="count">Number of items to drop</param>
        /// <returns>Return the first element of the stack</returns>
        public override int Drop(int count = 0)
        {
            return NeoVM.StackItems_Drop(_handle, count);
        }
        /// <summary>
        /// Pop object from the stack
        /// </summary>
        /// <param name="free">True for free object</param>
        /// <returns>Return the first element of the stack</returns>
        public override IStackItem Pop()
        {
            IntPtr ptr = NeoVM.StackItems_Pop(_handle);

            if (ptr == IntPtr.Zero)
                throw (new IndexOutOfRangeException());

            return Engine.ConvertFromNative(ptr);
        }
        /// <summary>
        /// Push objet to the stack
        /// </summary>
        /// <param name="item">Object</param>
        public override void Push(IStackItem item)
        {
            NeoVM.StackItems_Push(_handle, ((INativeStackItem)item).Handle);
        }
        /// <summary>
        /// Try to obtain the element at `index` position, without consume them
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="obj">Object</param>
        /// <returns>Return tru eif object exists</returns>
        public override bool TryPeek(int index, out IStackItem obj)
        {
            if (index < 0)
            {
                obj = null;
                return false;
            }

            IntPtr ptr = NeoVM.StackItems_Peek(_handle, index);

            if (ptr == IntPtr.Zero)
            {
                obj = null;
                return false;
            }

            obj = Engine.ConvertFromNative(ptr);
            return true;
        }
        /// <summary>
        /// Try Pop object casting to this type
        /// </summary>
        /// <typeparam name="TStackItem">Object type</typeparam>
        /// <param name="item">Item</param>
        /// <returns>Return false if it is something wrong</returns>
        public override bool TryPop<TStackItem>(out TStackItem item)
        {
            IntPtr ptr = NeoVM.StackItems_Pop(_handle);

            if (ptr == IntPtr.Zero)
            {
                item = default(TStackItem);
                return false;
            }

            IStackItem ret = Engine.ConvertFromNative(ptr);

            if (ret is TStackItem ts)
            {
                item = (TStackItem)ret;
                return true;
            }

            item = default(TStackItem);
            return false;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="handle">Handle</param>
        internal StackItemStack(IExecutionEngine engine, IntPtr handle) : base(engine)
        {
            _handle = handle;
            Engine = (ExecutionEngine)engine;

            if (handle == IntPtr.Zero)
                throw new ExternalException();
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return Count.ToString();
        }

        /// <summary>
        /// Free resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            _handle = IntPtr.Zero;
        }
    }
}