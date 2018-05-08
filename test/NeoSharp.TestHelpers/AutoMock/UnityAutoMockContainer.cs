using System;
using System.Collections.Generic;
using System.Diagnostics;
using Moq;
using Unity;
using Unity.Lifetime;

namespace NeoSharp.TestHelpers.AutoMock
{
    public class UnityAutoMockContainer : UnityContainer, IAutoMockContainer
    {
        private readonly IDictionary<Type, AsExpression> _asExpressions;

        public UnityAutoMockContainer(MockRepository mockRepository)
        {
            AddExtension(new UnityAutoMoqExtension(mockRepository, this));

            _asExpressions = new Dictionary<Type, AsExpression>();
        }

        [DebuggerStepThrough]
        public T Create<T>()
        {
            return (T)Resolve(typeof(T), null);
        }

        [DebuggerStepThrough]
        public T Get<T>() where T : class
        {
            return this.Resolve<T>();
        }

        [DebuggerStepThrough]
        public Mock<T> GetMock<T>() where T : class
        {
            return Mock.Get(Create<T>());
        }

        [DebuggerStepThrough]
        public void Register<TService, TImplementation>() where TImplementation : TService
        {
            this.RegisterType<TService, TImplementation>(new ContainerControlledLifetimeManager());
        }

        [DebuggerStepThrough]
        public void Register<TService>(TService instance)
        {
            this.RegisterInstance(instance);
        }

        [DebuggerStepThrough]
        public void Register<TService>(TService instance, string name)
        {
            this.RegisterInstance(name, instance);
        }

        [DebuggerStepThrough]
        internal AsExpression GetInterfaceImplementations(Type t)
        {
            return _asExpressions.ContainsKey(t) ? _asExpressions[t] : null;
        }
    }
}
