using System;
using System.Numerics;

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
            {
                throw new ArgumentOutOfRangeException();
            }

            if (obj is TStackItem ts) return ts;

            throw new FormatException();
        }

        /// <summary>
        /// Pop object casting to this type
        /// </summary>
        /// <typeparam name="TStackItem">Object type</typeparam>
        /// <returns>Return object</returns>
        public TStackItem Pop<TStackItem>() where TStackItem : IStackItem
        {
            if (Pop() is TStackItem ts) return ts;

            throw new FormatException();
        }

        /// <summary>
        /// Try Pop object casting to this type
        /// </summary>
        /// <typeparam name="TStackItem">Object type</typeparam>
        /// <param name="item">Item</param>
        /// <returns>Return false if it is something wrong</returns>
        public abstract bool TryPop<TStackItem>(out TStackItem item) where TStackItem : IStackItem;

        /// <summary>
        /// Try pop byte array
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return false if is something wrong or is not convertible to ByteArray</returns>
        public bool TryPop(out byte[] value)
        {
            if (TryPop<IStackItem>(out var item))
            {
                using (item)
                {
                    if (item.CanConvertToByteArray)
                    {
                        value = item.ToByteArray();
                        return true;
                    }
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// Try pop BigInteger
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return false if is something wrong or is not convertible to BigInteger</returns>
        public bool TryPop(out BigInteger value)
        {
            if (TryPop<IStackItem>(out var item))
            {
                using (item)
                {
                    if (item is IIntegerStackItem integer)
                    {
                        value = integer.Value;
                        return true;
                    }
                    else if (item.CanConvertToByteArray)
                    {
                        value = new BigInteger(item.ToByteArray());
                        return true;
                    }
                }
            }

            value = BigInteger.Zero;
            return false;
        }

        /// <summary>
        /// Try pop bool
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return false if is something wrong or is not convertible to bool</returns>
        public bool TryPop(out bool value)
        {
            if (TryPop<IStackItem>(out var item))
            {
                using (item)
                {
                    if (item is IBooleanStackItem integer)
                    {
                        value = integer.Value;
                        return true;
                    }
                    else if (item.CanConvertToByteArray)
                    {
                        var ret = item.ToByteArray();
                        value = ret != null && ret.Length != 0;

                        return true;
                    }
                }
            }

            value = false;
            return false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        protected IStackItemsStack(IExecutionEngine engine) : base(engine) { }
    }
}