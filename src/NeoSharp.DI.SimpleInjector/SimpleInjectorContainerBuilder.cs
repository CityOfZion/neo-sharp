using System;
using NeoSharp.Core.DI;
using SimpleInjector;

namespace NeoSharp.DI.SimpleInjector
{
    public class SimpleInjectorContainerBuilder : IContainerBuilder
    {
        private readonly Container _container;

        public SimpleInjectorContainerBuilder()
        {
            _container = new Container();
        }

        public void RegisterSingleton<TImplementation>()
            where TImplementation : class
        {
            _container.Register<TImplementation>(Lifestyle.Singleton);
        }

        public void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.Register<TService, TImplementation>(Lifestyle.Singleton);
        }

        public void RegisterSingleton<TService>(Func<TService> instanceCreator)
            where TService : class
        {
            _container.Register(instanceCreator, Lifestyle.Singleton);
        }

        public void RegisterSingleton(Type service, Type implementation)
        {
            _container.Register(service, implementation, Lifestyle.Singleton);
        }

        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.Register<TService, TImplementation>();
        }

        public void Register(Type service, Type implementation)
        {
            _container.Register(service, implementation);
        }

        public void Register<TService>(TService configuration)
            where TService : class
        {
            _container.RegisterInstance(configuration);
        }

        public void RegisterInstanceCreator<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.RegisterInstanceCreator<TService, TImplementation>(Lifestyle.Transient);
        }

        public void RegisterModule<TModule>() where TModule : class, IModule, new()
        {
            var module = new TModule();

            module.Register(this);
        }

        public IContainer Build()
        {
            _container.Verify();

            return new SimpleInjectorContainer(_container);
        }
    }
}
