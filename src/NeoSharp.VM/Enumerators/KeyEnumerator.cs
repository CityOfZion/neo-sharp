using System;
using System.Collections;
using System.Collections.Generic;

namespace NeoSharp.VM
{
    public class KeyEnumerator : IEnumerator<StackItemBase>
    {
        private readonly IEnumerator<KeyValuePair<StackItemBase, StackItemBase>> _enumerator;

        public KeyEnumerator(IEnumerator<KeyValuePair<StackItemBase, StackItemBase>> enumerator)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
        }

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();

        public StackItemBase CurrentKey => _enumerator.Current.Key;

        public StackItemBase Current => _enumerator.Current.Value;

        object IEnumerator.Current => Current;

        public void Dispose() => _enumerator.Dispose();
    }
}