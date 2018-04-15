using System;
using System.Collections.Generic;

namespace NeoSharp.TestHelpers.AutoMock
{
    public class AsExpression
    {
        private readonly List<Type> _implements = new List<Type>();

        public AsExpression(Type implements)
        {
            _implements.Add(implements);
        }

        public AsExpression As<T>() where T : class
        {
            _implements.Add(typeof(T));
            return this;
        }

        internal IEnumerable<Type> GetImplementations()
        {
            return _implements;
        }
    }
}
