using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoSharp.VM
{
    public abstract class IArrayStackItem : IStackItem, IList<IStackItem>, ICollection, IEquatable<IArrayStackItem>
    {
        public override bool CanConvertToByteArray => false;

        public override byte[] ToByteArray() => throw new NotImplementedException();

        /// <summary>
        /// Count
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Index
        /// </summary>
        /// <param name="index">Position</param>
        /// <returns>Returns the StackItem element</returns>
        public abstract IStackItem this[int index] { get; set; }

        /// <summary>
        /// IsStruct
        /// </summary>
        public bool IsStruct => Type == EStackItemType.Struct;

        /// <summary>
        /// IsReadOnly
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// IsSynchronized
        /// </summary>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        /// SyncRoot
        /// </summary>
        object ICollection.SyncRoot => this;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="isStruct">Is struct</param>
        public IArrayStackItem(IExecutionEngine engine, bool isStruct) : base(engine, isStruct ? EStackItemType.Struct : EStackItemType.Array) { }

        /// <summary>
        /// Get raw object
        /// </summary>
        /// <returns>Raw object</returns>
        public override object GetRawObject()
        {
            var size = Count;
            var ret = new object[size];

            for (var x = 0; x < size; x++)
            {
                using (var item = this[x])
                {
                    ret[x] = item.GetRawObject();
                }
            }

            return ret;
        }

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public bool Equals(IArrayStackItem other)
        {
            if (other == null) return false;
            if (other == this) return true;
            if (other.Type != Type) return false;

            return this.SequenceEqual(other);
        }

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public override bool Equals(IStackItem other)
        {
            if (!(other is IArrayStackItem st)) return false;
            if (st.Type != Type) return false;

            return this.SequenceEqual(st);
        }

        #region Write

        public abstract void Add(IStackItem item);

        public abstract void Add(params IStackItem[] items);

        public abstract void Add(IEnumerable<IStackItem> items);

        public abstract void Clear();

        public abstract void Insert(int index, IStackItem item);

        public abstract void Set(int index, IStackItem item);

        public abstract void RemoveAt(int index);

        public bool Remove(IStackItem item)
        {
            var ix = IndexOf(item);
            if (ix < 0) return false;

            RemoveAt(ix);
            return true;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns>Returns copy of this object</returns>
        public IArrayStackItem Clone()
        {
            if (Type == EStackItemType.Struct)
            {
                // Struct logic

                var newArray = new List<IStackItem>(Count);

                foreach (var it in this)
                {
                    if (it is IArrayStackItem s && it.Type == EStackItemType.Struct)
                        newArray.Add(s.Clone());
                    else
                        newArray.Add(it);
                }

                return Engine.CreateStruct(newArray);
            }
            else
            {
                return Engine.CreateArray(this);
            }
        }

        #endregion

        #region Read

        public abstract int IndexOf(IStackItem item);

        public bool Contains(IStackItem item) => IndexOf(item) >= 0;

        #endregion

        #region IEnumerable

        public void CopyTo(Array array, int index)
        {
            foreach (var item in this) array.SetValue(item, index++);
        }

        public void CopyTo(IStackItem[] array, int index)
        {
            foreach (var item in this) array.SetValue(item, index++);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IStackItem> GetEnumerator()
        {
            for (int x = 0, m = Count; x < m; x++)
            {
                yield return this[x];
            }
        }

        #endregion

        /// <summary>
        /// String representation
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("[");

            var first = true;

            foreach (var it in this)
            {
                if (first) first = false;
                else sb.Append(",");

                sb.Append(it.ToString());
                it.Dispose();
            }

            sb.Append("]");

            return sb.ToString();
        }
    }
}