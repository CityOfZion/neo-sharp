using SimpleInjector;
using Microsoft.Extensions.Logging;
using System;
using NLog.Extensions.Logging;

namespace NeoSharp.Logging.DI
{
    public static class LoggingPackage
    {
        public static void RegisterServices(Container container)
        {
            container.Register(ConfigureLogger, Lifestyle.Singleton);
            container.Register(typeof(ILogger<>), typeof(LoggerAdapter<>), Lifestyle.Singleton);
        }

        private static ILoggerFactory ConfigureLogger()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            loggerFactory.ConfigureNLog("nlog.config");            

            return loggerFactory;
        }        
    }

    /// <summary>
    ///  This Adapter class allows us to use Simple Injector with Microsoft.Extensions.Logging and ILogger<T>
    /// </summary>
    public class LoggerAdapter<T> : ILogger<T>
    {
        private readonly ILogger _logger;

        public LoggerAdapter(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<T>();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return _logger.BeginScope(state);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _logger.IsEnabled(logLevel);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }

}
