using System;
using Newtonsoft.Json;

namespace NeoSharp.VM
{
    public abstract class IStackItem : IEquatable<IStackItem>, IDisposable
    {
        /// <summary>
        /// Can convert to byte array
        /// </summary>
        [JsonIgnore]
        public abstract bool CanConvertToByteArray { get; }
        /// <summary>
        /// Engine
        /// </summary>
        [JsonIgnore]
        protected readonly IExecutionEngine Engine;
        /// <summary>
        /// Type
        /// </summary>
        public readonly EStackItemType Type;
        /// <summary>
        /// Is Disposed
        /// </summary>
        [JsonIgnore]
        public abstract bool IsDisposed { get; }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="type">Type</param>
        protected IStackItem(IExecutionEngine engine, EStackItemType type)
        {
            Type = type;
            Engine = engine;
        }

        /// <summary>
        /// Convert to Byte array
        /// </summary>
        public abstract byte[] ToByteArray();

        /// <summary>
        /// Get Raw object
        /// </summary>
        /// <returns>Raw object</returns>
        public abstract object GetRawObject();

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other">Other item</param>
        public virtual bool Equals(IStackItem other)
        {
            if (other == null) return false;

            return ReferenceEquals(this, other);
        }

        #region IDisposable Support

        /// <summary>
        /// Dispose logic
        /// </summary>
        /// <param name="disposing">False for cleanup native objects</param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        /// Destructor
        /// </summary>
        ~IStackItem()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}