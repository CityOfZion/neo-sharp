using System;

namespace NeoSharp.Application.DI
{
    public interface ISimpleInjectorWrapper
    {
        void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        void RegisterSingleton<TImplementation>()
            where TImplementation : class;

        void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        void RegisterSingleton(Type service, Type implementation);

        void Register<TService>(TService configuration)
            where TService : class;

        void RegisterTransientInstance<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        TEntity Resolve<TEntity>()
            where TEntity : class;

        void Verify();
    }
}
