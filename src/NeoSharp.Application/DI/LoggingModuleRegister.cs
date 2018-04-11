using Microsoft.Extensions.Logging;
using NeoSharp.Core.Logging;
using NeoSharp.Logging.NLog;

namespace NeoSharp.Application.DI
{
    public class LoggingModuleRegister 
    {
        public void Register(ISimpleInjectorWrapper container)
        {
            container.RegisterSingleton<ILoggerFactory, NLogLoggerFactory>();
            container.RegisterSingleton(typeof(ILogger<>), typeof(LoggerAdapter<>));
        }
    }
}