using System;
using System.Collections.Generic;
using System.Linq;
using NeoSharp.Core.DI;
using SimpleInjector;

namespace NeoSharp.DI.SimpleInjector
{
    public class SimpleInjectorContainerBuilder : IContainerBuilder
    {
        private readonly Container _container;
        private readonly SimpleInjectorContainer _containerAdapter;

        public event Action<IContainer> OnBuild;

        public SimpleInjectorContainerBuilder()
        {
            _container = new Container();
            _containerAdapter = new SimpleInjectorContainer(_container);

            RegisterInstance<IContainer>(_containerAdapter);
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

        public void Register(Type service, IEnumerable<Type> implementations)
        {
            _container.Register(service, implementations);
        }

        public void RegisterCollection(Type service, IEnumerable<Type> implementations)
        {
            var registrations = implementations
                .Select(x => Lifestyle.Singleton.CreateRegistration(x, _container))
                .ToArray();

            _container.Collection.Register(service, registrations);
        }

        public void Register<TService>(TService configuration)
            where TService : class
        {
            _container.RegisterInstance(configuration);
        }

        public void RegisterInstance<TService>(TService instance) where TService : class
        {
            _container.RegisterInstance(instance);
        }

        public void RegisterInstanceCreator<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.RegisterInstanceCreator<TService, TImplementation>(Lifestyle.Transient);
        }

        public void RegisterInstanceCreator<TService>(Func<TService> instanceCreator) where TService : class
        {
            _container.RegisterSingleton(instanceCreator);
        }

        public void RegisterInstanceCreator<TService>(Func<IContainer, TService> instanceCreator) where TService : class
        {
            _container.RegisterSingleton(() => instanceCreator(_containerAdapter));
        }

        public void RegisterModule<TModule>() where TModule : class, IModule, new()
        {
            var module = new TModule();

            module.Register(this);
        }

        public IContainer Build()
        {
            _container.Verify();

            OnBuild?.Invoke(_containerAdapter);

            return _containerAdapter;
        }
    }
}
