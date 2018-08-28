using System;
using System.Collections;
using System.Collections.Generic;

namespace NeoSharp.VM
{
    public abstract class IStack<T> : IEnumerable<T>, IDisposable
    {
        /// <summary>
        /// Return the number of items in the stack
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Drop object from the stack
        /// </summary>
        /// <param name="count">Number of items to drop</param>
        /// <returns>Return the first element of the stack</returns>
        public abstract int Drop(int count = 0);

        /// <summary>
        /// Pop object from the stack
        /// </summary>
        /// <param name="free">True for free object</param>
        /// <returns>Return the first element of the stack</returns>
        public abstract T Pop();

        /// <summary>
        /// Push objet to the stack
        /// </summary>
        /// <param name="item">Object</param>
        public abstract void Push(T item);

        /// <summary>
        /// Try to obtain the element at `index` position, without consume them
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="obj">Object</param>
        /// <returns>Return tru eif object exists</returns>
        public abstract bool TryPeek(int index, out T obj);

        /// <summary>
        /// Obtain the element at `index` position, without consume them
        /// </summary>
        /// <param name="index">Index</param>
        /// <returns>Return object</returns>
        public T Peek(int index = 0)
        {
            if (!TryPeek(index, out T obj))
                throw new ArgumentOutOfRangeException();

            return obj;
        }
        
        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            return Count.ToString();
        }

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            for (int x = 0, m = Count; x < m; x++)
                yield return Peek(x);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int x = 0, m = Count; x < m; x++)
                yield return Peek(x);
        }

        #endregion

        #region IDisposable Support

        /// <summary>
        /// Dispose logic
        /// </summary>
        /// <param name="disposing">False for cleanup native objects</param>
        protected virtual void Dispose(bool disposing) { }

        /// <summary>
        /// Destructor
        /// </summary>
        ~IStack()
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