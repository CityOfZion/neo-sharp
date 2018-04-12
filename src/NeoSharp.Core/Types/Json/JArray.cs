using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeoSharp.Core.Types.Json
{
    public class JArray : JObject, IList<JObject>
    {
        private List<JObject> _items = new List<JObject>();

        public JArray(params JObject[] items) : this((IEnumerable<JObject>)items)
        {
        }

        public JArray(IEnumerable<JObject> items)
        {
            _items.AddRange(items);
        }

        public JObject this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(JObject item)
        {
            _items.Add(item);
        }

        public void Clear()
        {
            _items.Clear();
        }

        public bool Contains(JObject item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(JObject[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public IEnumerator<JObject> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(JObject item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, JObject item)
        {
            _items.Insert(index, item);
        }

        internal new static JArray Parse(TextReader reader)
        {
            SkipSpace(reader);
            if (reader.Read() != '[') throw new FormatException();
            SkipSpace(reader);
            var array = new JArray();
            while (reader.Peek() != ']')
            {
                if (reader.Peek() == ',') reader.Read();
                var obj = JObject.Parse(reader);
                array._items.Add(obj);
                SkipSpace(reader);
            }
            reader.Read();
            return array;
        }

        public bool Remove(JObject item)
        {
            return _items.Remove(item);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append('[');
            foreach (var item in _items)
            {
                if (item == null)
                    sb.Append("null");
                else
                    sb.Append(item);
                sb.Append(',');
            }
            if (_items.Count == 0)
            {
                sb.Append(']');
            }
            else
            {
                sb[sb.Length - 1] = ']';
            }
            return sb.ToString();
        }
    }
}
