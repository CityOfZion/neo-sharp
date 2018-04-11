using NeoSharp.Core;

namespace NeoSharp.Application.DI
{
    public class ApplicationBootstrapper
    {
        private readonly SimpleInjectorWrapper _container;

        public ApplicationBootstrapper()
        {
            this._container = new SimpleInjectorWrapper();
        }

        public void RegisterModules()
        {
            new ClientModuleRegister().Register(this._container);
            new ConfigurationModuleRegister().Register(this._container);
            new LoggingModuleRegister().Register(this._container);
            new NetworkModuleRegister().Register(this._container);

            this._container.Verify();
        }

        public void Start(string[] args)
        {
            var bootstrapper = this._container.Resolve<IBootstrapper>();
            bootstrapper.Start(args);
        }
    }
}
