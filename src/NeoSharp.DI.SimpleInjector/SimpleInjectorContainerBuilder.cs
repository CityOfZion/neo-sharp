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
            this._container = new Container();
        }

        public void RegisterSingleton<TImplementation>()
            where TImplementation : class
        {
            this._container.Register<TImplementation>(Lifestyle.Singleton);
        }

        public void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            this._container.Register<TService, TImplementation>(Lifestyle.Singleton);
        }

        public void RegisterSingleton<TService>(Func<TService> instanceCreator)
            where TService : class
        {
            this._container.Register(instanceCreator, Lifestyle.Singleton);
        }

        public void RegisterSingleton(Type service, Type implementation)
        {
            this._container.Register(service, implementation, Lifestyle.Singleton);
        }

        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            this._container.Register<TService, TImplementation>();
        }

        public void Register(Type service, Type implementation)
        {
            this._container.Register(service, implementation);
        }

        public void Register<TService>(TService configuration)
            where TService : class
        {
            this._container.RegisterInstance(configuration);
        }

        public void RegisterInstanceCreator<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            this._container.RegisterInstanceCreator<TService, TImplementation>(Lifestyle.Transient);
        }

        public void RegisterModule<TModule>() where TModule : class, IModule, new()
        {
            var module = new TModule();

            module.Register(this);
        }

        public IContainer Build()
        {
            this._container.Verify();

            return new SimpleInjectorContainer(_container);
        }
    }
}
