using SimpleInjector;
using NeoSharp.Modules;

namespace NeoSharp.Config.DI
{
    public static class ConfigPackage
    {
        public static void RegisterServices(Container container)
        {
            container.Register<IConfigManager, ConfigManager>(Lifestyle.Singleton);
        }     
    }

}
