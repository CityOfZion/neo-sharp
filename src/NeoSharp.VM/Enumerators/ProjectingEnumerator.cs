using System;
using System.Collections;
using System.Collections.Generic;

namespace NeoSharp.VM
{
    public class ProjectingEnumerator<TSourceEnumerator> : IEnumerator<StackItemBase>
        where TSourceEnumerator : class, IEnumerator<StackItemBase>
    {
        private readonly TSourceEnumerator _enumerator;
        private readonly Func<TSourceEnumerator, StackItemBase> _project;

        public ProjectingEnumerator(TSourceEnumerator enumerator, Func<TSourceEnumerator, StackItemBase> project)
        {
            _enumerator = enumerator ?? throw new ArgumentNullException(nameof(enumerator));
            _project = project ?? throw new ArgumentNullException(nameof(project));
        }

        public bool MoveNext() => _enumerator.MoveNext();

        public void Reset() => _enumerator.Reset();

        public StackItemBase Current => _project(_enumerator);

        object IEnumerator.Current => Current;

        public void Dispose() => _enumerator.Dispose();
    }
}