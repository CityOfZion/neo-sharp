using System;
using NeoSharp.Core.DI;
using SimpleInjector;

namespace NeoSharp.DI.SimpleInjector
{
    public class SimpleInjectorContainer : IContainer
    {
        private readonly Container _container;

        internal SimpleInjectorContainer(Container container)
        {
            _container = container;
        }

        public object Resolve(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }

        public TEntity Resolve<TEntity>() where TEntity : class
        {
            return _container.GetInstance<TEntity>();
        }
    }
}