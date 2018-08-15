using System;
using System.Collections.Generic;
using NeoSharp.VM.Interop.Extensions;
using NeoSharp.VM.Interop.Native;
using Newtonsoft.Json;

namespace NeoSharp.VM.Interop.Types.StackItems
{
    public class MapStackItem : IMapStackItem, INativeStackItem
    {
        #region Private fields

        /// <summary>
        /// Native Handle
        /// </summary>
        private IntPtr _handle;

        /// <summary>
        /// Engine
        /// </summary>
        [JsonIgnore]
        private readonly new ExecutionEngine Engine;

        #endregion

        #region Public fields

        public override bool CanConvertToByteArray => false;

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

        /// <summary>
        /// Count
        /// </summary>
        public override int Count => NeoVM.MapStackItem_Count(_handle);

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        internal MapStackItem(ExecutionEngine engine) : this(engine, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="value">Value</param>
        internal MapStackItem(ExecutionEngine engine, Dictionary<IStackItem, IStackItem> value) : base(engine)
        {
            Engine = engine;
            _handle = this.CreateNativeItem();

            if (value == null) return;

            foreach (KeyValuePair<IStackItem, IStackItem> pair in value)
                Set(pair.Key, pair.Value);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="handle">Handle</param>
        internal MapStackItem(ExecutionEngine engine, IntPtr handle) : base(engine)
        {
            Engine = engine;
            _handle = handle;
        }

        #endregion

        #region Write

        public override bool Remove(IStackItem key)
        {
            return NeoVM.MapStackItem_Remove(_handle, key == null ? IntPtr.Zero : ((INativeStackItem)key).Handle) == NeoVM.TRUE;
        }

        public override void Set(KeyValuePair<IStackItem, IStackItem> item)
        {
            Set(item.Key, item.Value);
        }

        public override void Set(IStackItem key, IStackItem value)
        {
            NeoVM.MapStackItem_Set
                (
                _handle,
                key == null ? IntPtr.Zero : ((INativeStackItem)key).Handle,
                value == null ? IntPtr.Zero : ((INativeStackItem)value).Handle
                );
        }

        public override void Clear()
        {
            NeoVM.MapStackItem_Clear(_handle);
        }

        #endregion

        #region Read

        public override bool ContainsKey(IStackItem key)
        {
            return NeoVM.MapStackItem_Get(_handle, key == null ? IntPtr.Zero : ((INativeStackItem)key).Handle) != IntPtr.Zero;
        }

        public override bool TryGetValue(IStackItem key, out IStackItem value)
        {
            var ret = NeoVM.MapStackItem_Get(_handle, key == null ? IntPtr.Zero : ((INativeStackItem)key).Handle);

            if (ret == IntPtr.Zero)
            {
                value = null;
                return false;
            }

            value = Engine.ConvertFromNative(ret);
            return true;
        }

        #endregion

        #region Enumerables

        public override IEnumerable<IStackItem> GetKeys()
        {
            for (int x = 0, c = Count; x < c; x++)
            {
                yield return Engine.ConvertFromNative(NeoVM.MapStackItem_GetKey(_handle, x));
            }
        }

        public override IEnumerable<IStackItem> GetValues()
        {
            for (int x = 0, c = Count; x < c; x++)
            {
                yield return Engine.ConvertFromNative(NeoVM.MapStackItem_GetValue(_handle, x));
            }
        }

        public override IEnumerator<KeyValuePair<IStackItem, IStackItem>> GetEnumerator()
        {
            for (int x = 0, c = Count; x < c; x++)
            {
                var key = Engine.ConvertFromNative(NeoVM.MapStackItem_GetKey(_handle, x));
                var value = Engine.ConvertFromNative(NeoVM.MapStackItem_GetValue(_handle, x));

                yield return new KeyValuePair<IStackItem, IStackItem>(key, value);
            }
        }

        #endregion

        public override byte[] ToByteArray() => throw new NotImplementedException();

        public byte[] GetNativeByteArray() => null;

        #region IDisposable Support

        protected override void Dispose(bool disposing)
        {
            if (_handle == IntPtr.Zero) return;

            NeoVM.StackItem_Free(ref _handle);
        }

        #endregion
    }
}