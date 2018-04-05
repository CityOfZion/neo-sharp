using System;
using System.Collections.Generic;

namespace NeoSharp.TestHelpers.AutoMock
{
    public class AsExpression
    {
        private readonly List<Type> implements = new List<Type>();

        public AsExpression(Type implements)
        {
            this.implements.Add(implements);
        }

        public AsExpression As<T>() where T : class
        {
            implements.Add(typeof(T));
            return this;
        }

        internal IEnumerable<Type> GetImplementations()
        {
            return implements;
        }
    }
}
