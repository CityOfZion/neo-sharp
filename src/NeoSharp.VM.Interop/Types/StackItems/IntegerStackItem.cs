using System;
using System.Numerics;
using NeoSharp.VM.Interop.Extensions;
using NeoSharp.VM.Interop.Native;
using Newtonsoft.Json;

namespace NeoSharp.VM.Interop.Types.StackItems
{
    public class IntegerStackItem : IIntegerStackItem, INativeStackItem
    {
        #region Private fields

        /// <summary>
        /// Native Handle
        /// </summary>
        private IntPtr _handle;

        #endregion

        #region Public fields

        /// <summary>
        /// Native Handle
        /// </summary>
        [JsonIgnore]
        public IntPtr Handle => _handle;

        /// <summary>
        /// Is Disposed
        /// </summary>
        [JsonIgnore]
        public override bool IsDisposed => _handle == IntPtr.Zero;

        /// <summary>
        /// Type
        /// </summary>
        public new EStackItemType Type => base.Type;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="data">Data</param>
        internal IntegerStackItem(ExecutionEngine engine, int data) : base(engine, new BigInteger(data))
        {
            _handle = this.CreateNativeItem();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="data">Data</param>
        internal IntegerStackItem(ExecutionEngine engine, long data) : base(engine, new BigInteger(data))
        {
            _handle = this.CreateNativeItem();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="data">Data</param>
        internal IntegerStackItem(ExecutionEngine engine, byte[] data) : base(engine, new BigInteger(data))
        {
            _handle = this.CreateNativeItem();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="data">Data</param>
        internal IntegerStackItem(ExecutionEngine engine, BigInteger data) : base(engine, data)
        {
            _handle = this.CreateNativeItem();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="handle">Handle</param>
        /// <param name="value">Raw value</param>
        internal IntegerStackItem(ExecutionEngine engine, IntPtr handle, byte[] value) :
            base(engine, new BigInteger(value))
        {
            _handle = handle;
        }

        public byte[] GetNativeByteArray() => Value.ToByteArray();

        #region IDisposable Support

        protected override void Dispose(bool disposing)
        {
            if (_handle == IntPtr.Zero) return;

            NeoVM.StackItem_Free(ref _handle);
        }

        #endregion
    }
}