using System;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace NeoSharp.VM.Interop.Types.Collections
{
    public class ExecutionContextStack : IStack<IExecutionContext>
    {
        /// <summary>
        /// Native handle
        /// </summary>
        private IntPtr _handle;

        /// <summary>
        /// Engine
        /// </summary>
        [JsonIgnore]
        public new readonly ExecutionEngine Engine;

        /// <summary>
        /// Return the number of items in the stack
        /// </summary>
        public override int Count => NeoVM.ExecutionContextStack_Count(_handle);

        /// <summary>
        /// Drop object from the stack
        /// </summary>
        /// <param name="count">Number of items to drop</param>
        /// <returns>Return the first element of the stack</returns>
        public override int Drop(int count = 0)
        {
            return NeoVM.ExecutionContextStack_Drop(_handle, count);
        }

        #region Not implemented

        public override IExecutionContext Pop() => throw new NotImplementedException();
        public override void Push(IExecutionContext item) => throw new NotImplementedException();

        #endregion

        /// <summary>
        /// Try to obtain the element at `index` position, without consume them
        /// -1=Last , -2=Last-1 , -3=Last-2
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="obj">Object</param>
        /// <returns>Return tru eif object exists</returns>
        public override bool TryPeek(int index, out IExecutionContext obj)
        {
            var ptr = NeoVM.ExecutionContextStack_Peek(_handle, index);

            if (ptr == IntPtr.Zero)
            {
                obj = null;
                return false;
            }

            obj = new ExecutionContext(Engine, ptr);
            return true;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="handle">Handle</param>
        internal ExecutionContextStack(ExecutionEngine engine, IntPtr handle) : base(engine)
        {
            Engine = engine;
            _handle = handle;

            if (handle == IntPtr.Zero) throw new ExternalException();
        }

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString() => Count.ToString();

        /// <summary>
        /// Free resources
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            _handle = IntPtr.Zero;
        }
    }
}