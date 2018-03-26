using SimpleInjector;

namespace NeoSharp.Config.DI
{
    public static class ConfigPackage
    {
        public static void RegisterServices(Container container)
        {
            container.Register<IConfigLoader, ConfigLoader>(Lifestyle.Singleton);
        }     
    }

}
