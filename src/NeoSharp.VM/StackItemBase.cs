using System;
using Newtonsoft.Json;

namespace NeoSharp.VM
{
    public abstract class StackItemBase : IEquatable<StackItemBase>, IDisposable
    {
        /// <summary>
        /// Type
        /// </summary>
        public EStackItemType Type { get; }

        /// <summary>
        /// Is Disposed
        /// </summary>
        [JsonIgnore]
        public abstract bool IsDisposed { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type</param>
        protected StackItemBase(EStackItemType type)
        {
            Type = type;
        }

        /// <summary>
        /// Convert to Byte array
        /// </summary>
        public abstract byte[] ToByteArray();

        /// <summary>
        /// Get Raw object
        /// </summary>
        /// <returns>Raw object</returns>
        public abstract object ToObject();

        /// <inheritdoc />
        public virtual bool Equals(StackItemBase other)
        {
            if (other == null) return false;

            return ReferenceEquals(this, other);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return Equals(obj as StackItemBase);
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
        ~StackItemBase()
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