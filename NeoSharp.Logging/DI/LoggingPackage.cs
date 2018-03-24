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
            container.Register(typeof(ILogger<>), typeof(LoggingAdapter<>));
        }

        private static ILoggerFactory ConfigureLogger()
        {
            ILoggerFactory loggerFactory = new LoggerFactory();

            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            loggerFactory.ConfigureNLog("nlog.config");            

            return loggerFactory;
        }

        /// <summary>
        ///  This Adapter class lass allows us to use Simple Injector with Microsoft.Extensions.Logging and ILogger<T>
        /// </summary>
        public class LoggingAdapter<T> : ILogger<T>
        {
            private readonly ILogger _loggerAdapter;

            public LoggingAdapter(ILoggerFactory factory)
            {
                _loggerAdapter = factory.CreateLogger<T>();
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return _loggerAdapter.BeginScope(state);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return _loggerAdapter.IsEnabled(logLevel);
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                _loggerAdapter.Log(logLevel, eventId, state, exception, formatter);
            }
        }
    }

}
