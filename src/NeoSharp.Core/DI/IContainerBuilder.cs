using System;

namespace NeoSharp.Core.DI
{
    public interface IContainerBuilder
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

        void RegisterSingleton<TService>(Func<TService> instanceCreator)
            where TService : class;

        void Register<TService>(TService configuration)
            where TService : class;

        void Register(Type service, Type implementation);

        void RegisterInstanceCreator<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        void RegisterModule<TModule>()
            where TModule : class, IModule, new();

        IContainer Build();
    }
}