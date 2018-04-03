using Microsoft.Extensions.Logging;
using NeoSharp.Core.Logging;
using NeoSharp.Logging.NLog;
using SimpleInjector;

namespace NeoSharp.Application.DI
{
    public class LoggingPackage
    {
        public static void RegisterServices(Container container)
        {
            // logger
            container.Register<ILoggerFactory, NLogLoggerFactory>(Lifestyle.Singleton);
            container.Register(typeof(ILogger<>), typeof(LoggerAdapter<>), Lifestyle.Singleton);
        }
    }
}