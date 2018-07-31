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

        public bool TryResolve(Type parameterType, out object obj)
        {
            var ret = _container.GetRegistration(parameterType);

            if (ret != null)
            {
                obj = ret.GetInstance();
                return true;
            }

            obj = null;
            return false;
        }
    }
}
