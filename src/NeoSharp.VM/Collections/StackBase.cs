using System;
using System.Collections;
using System.Collections.Generic;

namespace NeoSharp.VM
{
    public abstract class StackBase<T> : IEnumerable<T>
    {
        /// <summary>
        /// Return the number of items in the stack
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Push the object to the stack
        /// </summary>
        /// <param name="item">Object</param>
        public abstract void Push(T item);

        /// <summary>
        /// Try peeking the object at `index` position from the stack
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="item">Object</param>
        /// <returns>Return false if something went wrong</returns>
        public abstract bool TryPeek(int index, out T item);

        /// <summary>
        /// Peek the object at `index` position from the stack
        /// </summary>
        /// <param name="index">Index</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>Return the peeked object from the stack</returns>
        public T Peek(int index = 0)
        {
            if (!TryPeek(index, out var item))
            {
                throw new InvalidOperationException();
            }

            return item;
        }

        /// <summary>
        /// Try popping the object from the stack
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <param name="item">Object</param>
        /// <returns>Return false if something went wrong</returns>
        public abstract bool TryPop(out T item);

        /// <summary>
        /// Pop the object from the stack
        /// </summary>
        /// <returns>Return the popped object from the stack</returns>
        public T Pop()
        {
            if (!TryPop(out var item))
            {
                throw new InvalidOperationException();
            }

            return item;
        }

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0, c = Count; i < c; i++)
            {
                yield return Peek(i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}