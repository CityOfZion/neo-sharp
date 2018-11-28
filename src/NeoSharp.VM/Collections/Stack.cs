using System;
using System.Numerics;

namespace NeoSharp.VM
{
    public abstract class Stack : StackBase<StackItemBase>
    {
        /// <summary>
        /// Obtain the element at `index` position, without consume them
        /// </summary>
        /// <param name="index">Index</param>
        /// <typeparam name="TStackItem">Object type</typeparam>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns>Return object</returns>
        public TStackItem Peek<TStackItem>(int index = 0) where TStackItem : StackItemBase
        {
            if (!TryPeek(index, out var obj))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (obj is TStackItem stackItem) return stackItem;

            throw new FormatException();
        }

        /// <summary>
        /// Pop object casting to this type
        /// </summary>
        /// <typeparam name="TStackItem">Object type</typeparam>
        /// <returns>Return object</returns>
        public TStackItem Pop<TStackItem>() where TStackItem : StackItemBase
        {
            if (Pop() is TStackItem stackItem) return stackItem;

            throw new FormatException();
        }

        /// <summary>
        /// Try Pop object casting to this type
        /// </summary>
        /// <typeparam name="TStackItem">Object type</typeparam>
        /// <param name="item">Item</param>
        /// <returns>Return false if it is something wrong</returns>
        public abstract bool TryPop<TStackItem>(out TStackItem item) where TStackItem : StackItemBase;

        /// <summary>
        /// Try pop byte array
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return false if is something wrong or is not convertible to ByteArray</returns>
        public bool TryPop(out byte[] value)
        {
            if (!TryPop<StackItemBase>(out var stackItem))
            {
                value = null;
                return false;
            }

            using (stackItem)
            {
                value = stackItem.ToByteArray();
                return value != null;
            }
        }

        /// <summary>
        /// Try pop BigInteger
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>Return false if is something wrong or is not convertible to BigInteger</returns>
        public bool TryPop(out BigInteger value)
        {
            if (TryPop<StackItemBase>(out var item))
            {
                using (item)
                {
                    if (item is IntegerStackItemBase integer)
                    {
                        value = integer.Value;
                        return true;
                    }

                    var array = item.ToByteArray();
                    if (array != null)
                    {
                        value = new BigInteger(array);
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
            if (!TryPop<StackItemBase>(out var item))
            {
                value = false;
                return false;
            }

            using (item)
            {
                if (item is BooleanStackItemBase integer)
                {
                    value = integer.Value;
                    return true;
                }

                var array = item.ToByteArray();
                value = array != null && array.Length != 0;

                return true;
            }
        }
    }
}