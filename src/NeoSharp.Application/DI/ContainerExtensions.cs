using System;
using SimpleInjector;

namespace NeoSharp.Application.DI
{
    public static class ContainerExtensions
    {
        public static void RegisterInstanceCreator<TService, TImpl>(
            this Container container, Lifestyle lifestyle = null)
            where TService : class
            where TImpl : class, TService
        {
            lifestyle = lifestyle ?? container.Options.DefaultLifestyle;
            var producer = lifestyle.CreateProducer<TService, TImpl>(container);
            container.RegisterInstance<Func<TService>>(producer.GetInstance);
        }
    }
}