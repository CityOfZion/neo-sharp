using System;
using NeoSharp.Core.Extensions;
using SimpleInjector;

namespace NeoSharp.Application.DI
{
    public class SimpleInjectorWrapper : ISimpleInjectorWrapper
    {
        private readonly Container _container;

        public SimpleInjectorWrapper()
        {
            this._container = new Container();
        }

        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            this._container.Register<TService, TImplementation>();
        }

        public void RegisterSingleton<TImplementation>()
            where TImplementation : class
        {
            this._container.Register<TImplementation>(Lifestyle.Singleton);
        }

        public void Register<TService>(TService configuration)
            where TService : class
        {
            this._container.RegisterInstance(configuration);
        }

        public void RegisterTransientInstance<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            this._container.RegisterInstanceCreator<TService, TImplementation>(Lifestyle.Transient);
        }

        public void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            this._container.Register<TService, TImplementation>(Lifestyle.Singleton);
        }

        public void RegisterSingleton(Type service, Type implementation)
        {
            this._container.Register(service, implementation, Lifestyle.Singleton);
        }


        public TEntity Resolve<TEntity>()
            where TEntity : class
        {
            return this._container.GetInstance<TEntity>();
        }

        public void Verify()
        {
            this._container.Verify();
        }
    }
}
