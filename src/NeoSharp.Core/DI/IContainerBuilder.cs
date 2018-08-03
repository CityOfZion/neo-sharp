using System;
using System.Collections.Generic;

namespace NeoSharp.Core.DI
{
    public interface IContainerBuilder
    {
        event Action<IContainer> OnBuild;
        
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

        void RegisterCollection(Type service, IEnumerable<Type> implementations);

        void RegisterInstance<TService>(TService instance)
            where TService : class;

        void RegisterInstanceCreator<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        void RegisterInstanceCreator<TService>(Func<TService> instanceCreator) where TService : class;

        void RegisterInstanceCreator<TService>(Func<IContainer, TService> instanceCreator)
            where TService : class;

        void RegisterModule<TModule>()
            where TModule : class, IModule, new();

        IContainer Build();
    }
}