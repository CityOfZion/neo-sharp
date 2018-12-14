using System;
using System.Collections;
using System.Collections.Generic;

namespace NeoSharp.VM
{
    public class ConcatenatedEnumerator : IEnumerator<StackItemBase>
    {
        private int _index;
        private readonly IEnumerator<StackItemBase>[] _enumerators;

        public ConcatenatedEnumerator(params IEnumerator<StackItemBase>[] enumerators)
        {
            _enumerators = enumerators ?? throw new ArgumentNullException(nameof(enumerators));

            if (enumerators.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(enumerators));

            _index = 0;
        }

        public bool MoveNext()
        {
            if (CurrentEnumerator.MoveNext()) return true;
            if (_index + 1 == _enumerators.Length) return false;

            _index++;

            return CurrentEnumerator.MoveNext();
        }

        public void Reset()
        {
            _index = 0;
            CurrentEnumerator.Reset();
        }

        public StackItemBase Current => CurrentEnumerator.Current;

        private IEnumerator<StackItemBase> CurrentEnumerator => _enumerators[_index];

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            foreach (var enumerator in _enumerators)
            {
                enumerator.Dispose();
            }
        }
    }
}