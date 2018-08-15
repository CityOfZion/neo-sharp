using System;

namespace NeoSharp.VM
{
    public abstract class IStackItemsStack : IStack<IStackItem>
    {
        /// <summary>
        /// Obtain the element at `index` position, without consume them
        /// </summary>
        /// <param name="index">Index</param>
        /// <typeparam name="TStackItem">Object type</typeparam>
        /// <returns>Return object</returns>
        public TStackItem Peek<TStackItem>(int index = 0) where TStackItem : IStackItem
        {
            if (!TryPeek(index, out IStackItem obj))
                throw new ArgumentOutOfRangeException();

            if (obj is TStackItem ts) return ts;

            throw (new FormatException());
        }
        /// <summary>
        /// Pop object casting to this type
        /// </summary>
        /// <typeparam name="TStackItem">Object type</typeparam>
        /// <returns>Return object</returns>
        public TStackItem Pop<TStackItem>() where TStackItem : IStackItem
        {
            if (Pop() is TStackItem ts) return ts;

            throw (new FormatException());
        }
        /// <summary>
        /// Try Pop object casting to this type
        /// </summary>
        /// <typeparam name="TStackItem">Object type</typeparam>
        /// <param name="item">Item</param>
        /// <returns>Return false if it is something wrong</returns>
        public abstract bool TryPop<TStackItem>(out TStackItem item) where TStackItem : IStackItem;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        protected IStackItemsStack(IExecutionEngine engine) : base(engine) { }
    }
}