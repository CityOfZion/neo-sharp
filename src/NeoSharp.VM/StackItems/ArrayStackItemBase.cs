using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoSharp.VM
{
    public abstract class ArrayStackItemBase : StackItemBase, IList<StackItemBase>, IEquatable<ArrayStackItemBase>
    {
        /// <summary>
        /// Count
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Index
        /// </summary>
        /// <param name="index">Position</param>
        /// <returns>Returns the StackItem element</returns>
        public abstract StackItemBase this[int index] { get; set; }

        /// <summary>
        /// IsStruct
        /// </summary>
        public bool IsStruct => Type == EStackItemType.Struct;

        /// <summary>
        /// IsReadOnly
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Is not possible to convert to byte array
        /// </summary>
        /// <returns>NULL</returns>
        public override byte[] ToByteArray() => null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="isStruct">Is struct</param>
        protected ArrayStackItemBase(bool isStruct) : base(isStruct ? EStackItemType.Struct : EStackItemType.Array) { }

        /// <summary>
        /// Get raw object
        /// </summary>
        /// <returns>Raw object</returns>
        public override object ToObject()
        {
            var size = Count;
            var ret = new object[size];

            for (var x = 0; x < size; x++)
            {
                using (var item = this[x])
                {
                    ret[x] = item.ToObject();
                }
            }

            return ret;
        }

        /// <summary>
        /// Is Equal
        /// </summary>
        /// <param name="other">Other</param>
        /// <returns>Return true if is equal</returns>
        public bool Equals(ArrayStackItemBase other)
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
        public override bool Equals(StackItemBase other)
        {
            if (!(other is ArrayStackItemBase st)) return false;
            if (st.Type != Type) return false;

            return this.SequenceEqual(st);
        }

        #region Write

        public abstract void Add(StackItemBase item);

        public abstract void Add(params StackItemBase[] items);

        public abstract void Add(IEnumerable<StackItemBase> items);

        public abstract void Clear();

        public abstract void Insert(int index, StackItemBase item);

        public abstract void Set(int index, StackItemBase item);

        public abstract void RemoveAt(int index);

        public bool Remove(StackItemBase item)
        {
            var ix = IndexOf(item);
            if (ix < 0) return false;

            RemoveAt(ix);
            return true;
        }

        #endregion

        #region Read

        public abstract int IndexOf(StackItemBase item);

        public bool Contains(StackItemBase item) => IndexOf(item) >= 0;

        #endregion

        #region IEnumerable

        public void CopyTo(Array array, int index)
        {
            foreach (var item in this) array.SetValue(item, index++);
        }

        public void CopyTo(StackItemBase[] array, int index)
        {
            foreach (var item in this) array.SetValue(item, index++);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<StackItemBase> GetEnumerator()
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

                sb.Append(it);
                it.Dispose();
            }

            sb.Append("]");

            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ArrayStackItemBase);
        }
    }
}